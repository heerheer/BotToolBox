using CommunityToolkit.Mvvm.ComponentModel;

namespace BotToolBox.Models.Chat;

[ObservableObject]
public partial class Message
{
    public string User { get; set; }
    public string Text { get; set; }
}