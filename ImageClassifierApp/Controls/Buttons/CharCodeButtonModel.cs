namespace ImageClassifierApp.Controls.Buttons
{
    public class CharCodeButtonModel : ButtonModel
    {
        public string CharCode { get; set; }

        public CharCodeButtonModel()
        {
            Button = new CharCodeButton()
            {
                DataContext = this
            };
        }
    }
}