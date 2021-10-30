using System.Windows.Forms;

namespace HelperSockets
{

    public delegate void updateLabelDelegate(string message);
    public class DisplayLabel : IDisplayMessage
    {
        private Label _label;
        private updateLabelDelegate _updateLabel;
        public DisplayLabel(Label label, updateLabelDelegate updateLabel)
        {
            _label = label;
            _updateLabel = updateLabel;
        }
        public void Display(string message)
        {
            _label.Invoke(_updateLabel, message);
        }
    }
}
