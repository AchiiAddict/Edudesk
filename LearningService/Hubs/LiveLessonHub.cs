using Microsoft.AspNetCore.SignalR;

namespace LearningService.Hubs
{
    // Canlı derslerdeki anket ve soru-cevap akışını yönetecek merkez
    public class LiveLessonHub : Hub
    {
        public async Task JoinLesson(string lessonId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lessonId);
            await Clients.Group(lessonId).SendAsync("ReceiveSystemMessage", "Canlı derse başarıyla bağlandınız.");
        }
        public async Task AskQuestion(string lessonId, string studentName, string question)
        {
            // Soru sadece o dersin odasındaki katılımcılara iletilir
            await Clients.Group(lessonId).SendAsync("ReceiveQuestion", studentName, question);
        }
        public async Task PublishPoll(string lessonId, string pollQuestion, string[] options)
        {
            // Eğitmen anketi tetiklediğinde, o dersteki tüm öğrencilerin ekranına düşer
            await Clients.Group(lessonId).SendAsync("ReceivePoll", pollQuestion, options);
        }

        public async Task LeaveLesson(string lessonId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lessonId);
        }
    }
}