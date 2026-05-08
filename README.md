# EduDesk — Çok Kiracılı Eğitim & Destek Yönetim Platformu

EduDesk, üniversiteler ve şirketler gibi kurumların kendi izole alanlarında hem canlı eğitim hem de teknik destek yönettiği bir SaaS platformudur. .NET 8 tabanlı mikroservis mimarisi üzerine inşa edilmiştir.

---

## Mimari Genel Bakış

```
[İstemci]
    │
    ▼
[API Gateway - YARP :5000]
    │
    ├──► [IdentityService  :8080]  → EduDesk_IdentityDb
    ├──► [SupportService   :8080]  → EduDesk_SupportDb
    └──► [LearningService  :8080]  → EduDesk_LearningDb
              │
              ▼
         [RabbitMQ :5672]
              │
              ▼
         [WorkerService]
```

Tüm servisler arası iletişim **RabbitMQ** mesaj kuyruğu üzerinden asenkron olarak gerçekleşir. Servisler birbirini doğrudan HTTP ile çağırmaz.

---

## Servisler

### IdentityService
Kullanıcı ve kurum (tenant) yönetimini üstlenir. Her kurumun verileri `TenantId` ile tamamen izole edilmiştir. Kullanıcı şifreleri **BCrypt** ile hashlenir, düz metin saklanmaz. Giriş başarılı olduğunda **15 dakika geçerli JWT Access Token** ve **7 gün geçerli Refresh Token** döner.

**Roller:** Admin, Instructor, Student, Support

### SupportService
Destek bileti oluşturma, listeleme ve SLA yönetimini üstlenir.

**Bracket Öncelik Algoritması:** Her bilete aşağıdaki formülle bir skor hesaplanır:

```
BracketScore = Urgency × RemainingSlaSaati × PlanMultiplier
```

PLAN		|		SLA		|		PLANMULT

Basic		|		48h		|		1

Pro		|		8h		|		2

Enterprise	|		2h		|		3



Skor ne kadar yüksekse bilet o kadar önce işleme alınır. Çözülen biletlerin skoru sıfırlanır.

**Idempotency:** Aynı `Idempotency-Key` header'ı ile ikinci kez gelen bilet oluşturma isteği reddedilir, böylece çift kayıt önlenir.

**SignalR Chat Hub:** Bilet içi gerçek zamanlı destek sohbeti WebSocket üzerinden çalışır. Bağlantı koptuğunda long-polling fallback devreye girer.

### LearningService
Ders ve kurs yönetimini üstlenir. Sertifika ve ders materyalleri için **HMAC-SHA256 imzalı, zaman sınırlı URL** üretilir.

**URL imzalama mekanizması:**
1. `filePath` ve `expiresAt` (Unix timestamp) birleştirilerek bir `rawData` oluşturulur.
2. Bu veri `HMACSHA256` ile imzalanır.
3. Oluşan URL: `/{filePath}?expires={timestamp}&signature={base64imza}` biçimindedir.
4. İstek geldiğinde hem süre hem imza doğrulanır; ikisi de geçerliyse erişim izni verilir.

**SignalR LiveLesson Hub:** Canlı derse katılan öğrenciler WebSocket ile bağlanır. Eğitmen ders içinde anlık soru yayınlayabilir, anket açabilir.

### WorkerService
Arka planda çalışan zamanlanmış işleri yönetir:

- **CertificateConsumer:** RabbitMQ kuyruğunu dinler, sertifika üretim talebini alır ve PDF üretim sürecini simüle eder.
- **SlaScannerWorker:** Her 1 dakikada bir çalışır, açık biletleri tarar ve SLA sınırına yaklaşanlar için uyarı eventi yayınlar.
- **DailyReportWorker:** Her 24 saatte bir çalışır, tenant bazlı günlük istatistik raporlarını e-posta kuyruğuna iletir.

### API Gateway (YARP)
Tüm servisler tek bir giriş noktasından (`localhost:5000`) yönlendirilir. Rate limiting (`fixed-policy`: 100 istek/dakika) ve CORS yapılandırması Gateway katmanında merkezi olarak yönetilir.

---

## Kurulum

### Gereksinimler
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Başlatma

```bash
git clone <repo-url>
cd EduDesk
docker compose up --build
```

Sistem tamamen ayağa kalkmak için yaklaşık 30-40 saniye bekleyin. SQL Server ve RabbitMQ sağlık kontrollerini geçmeden diğer servisler başlamaz.

---

## Port Bilgileri


SERVİS			|		PORT

API gateway		|		5000

RabbitMQ		|		15672

SQL Server		|		1433



## Swagger UI

SERVİS			|		URL

Identity API		|		http://localhost:5000/identity/swagger/index.html

Support API		|		http://localhost:5000/support/swagger/index.html

Learning API		|		http://localhost:5000/learning/swagger/index.html



## RabbitMQ Yönetim Paneli



URL:       http://localhost:15672
Kullanıcı: guest
Şifre:     guest





## Örnek API İstekleri

### Tenant Kaydı

\*\*http
POST http://localhost:5000/identity/api/tenant
Content-Type: application/json

{
"name": "X Üniversitesi",
"plan": "Pro"
}



### Kullanıcı Kaydı

\*\*http
POST http://localhost:5000/identity/api/user/register
Content-Type: application/json

{
"tenantId": "<tenant-guid>",
"fullName": "Doğukan Mavi",
"email": "dogu@test.com",
"passwordHash": "Dogu123",
"role": "Student"
}



### Giriş (JWT Al)

\*\*\*http
POST http://localhost:5000/identity/api/user/login
Content-Type: application/json

{
"email": "dogu@test.com",
"password": "Dogu123"
}



### Destek Bileti Oluştur (Idempotency-Key ile)

\*\*http
POST http://localhost:5000/support/api/ticket
Content-Type: application/json
Idempotency-Key: <benzersiz-uuid>

{
"tenantId": "<tenant-guid>",
"userId": "<user-guid>",
"subject": "Ders videosuna erişemiyorum",
"description": "Hata mesajı alıyorum.",
"urgency": 3,
"planMultiplier": 2
}



### Önceliklendirilmiş Bilet Kuyruğu

\*\*http
GET http://localhost:5000/support/api/ticket/queue



## Teknik Kararlar



**Neden Database-per-Service?**
Her mikroservis kendi bağımsız veritabanına sahiptir. Bu sayede bir servisin şeması değiştiğinde diğerleri etkilenmez ve servisler birbirinden tamamen bağımsız deploy edilebilir.

**Neden RabbitMQ + MassTransit?**
Servisler arası senkron HTTP çağrısı yapmak, bir servisin geçici olarak düşmesi durumunda tüm sistemi etkileyebilir. Asenkron mesaj kuyruğu ile bu bağımlılık ortadan kalkar. MassTransit, consumer tanımlamayı ve kuyruk yönetimini basitleştirir.

**Neden YARP?**
İstemcinin her servisi ayrı ayrı bilmesi gerekmez. Rate limiting ve CORS gibi cross-cutting concern'ler tek bir yerde yönetilir.

**Neden HMAC imzalı URL?**
Sertifika ve ders materyali gibi dosyalar herkese açık bırakılamaz, ancak her istek için kimlik doğrulama yapmak da maliyetlidir. Zaman sınırlı, imzalı URL ile dosya belirli bir süreliğine erişilebilir hale gelir; süre dolunca link geçersiz olur.



## Testler

bash
cd EduDesk.Tests
dotnet test



**Unit Testler (`TicketTests`):**

* SLA süresi dolmuş biletlerin skoru sıfır döner
* Aciliyeti yüksek biletler daha yüksek skor alır
* Bilet skoru beklenen aralıkta hesaplanır

**Unit Testler (`UrlSignerTests`):**

* Üretilen URL `expires` ve `signature` parametrelerini içerir
* Süresi geçmiş ya da sahte imzalı URL reddedilir

**Integration Testler (`SupportApiTests`):**

* `Idempotency-Key` olmadan bilet oluşturma `400 Bad Request` döner
* Geçerli istek ile bilet oluşturma `200 OK` döner



