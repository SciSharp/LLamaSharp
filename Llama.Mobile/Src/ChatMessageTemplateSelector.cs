using Microsoft.Maui.Controls;
namespace Llama.Mobile.Src
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate OtherTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = (Message)item;
            return message.Type == messageType.User ? UserTemplate : OtherTemplate;
        }
    }
}
