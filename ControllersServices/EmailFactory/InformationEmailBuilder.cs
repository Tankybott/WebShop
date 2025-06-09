using Services.EmailFactory.interfaces;
using Services.EmailFactory.models;


namespace Services.EmailFactory
{
    public class InformationEmailBuilder : IInformationEmailBuilder
    {
        public string Build(InformationEmailInput input)
        {
            var paragraphsHtml = string.Join(Environment.NewLine,
                input.Paragraphs.Select(p => $"<p style='font-size: 16px; color: #495057;'>{p}</p><br />"));

            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
                    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color: #343a40;'>{input.Title}</h2>
                        {paragraphsHtml}
                    </div>
                </body>
                </html>";
        }
    }
}
