using System.Text.Encodings.Web;
using Services.EmailFactory.interfaces;
using Services.EmailFactory.models;


namespace Services.EmailFactory
{

    public class LinkEmailBuilder : ILinkEmailBuilder
    {
        public string Build(LinkEmailInput input)
        {
            var encodedLink = HtmlEncoder.Default.Encode(input.Link);

            return $@"
        <html>
        <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
            <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                <h2 style='color: #343a40;'>Action Required</h2>
                <p style='font-size: 16px; color: #495057;'>{input.Message}</p>
                <p style='text-align: center; margin: 30px 0;'>
                    <a href='{encodedLink}'
                       style='background-color: #007bff; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;'>
                       Click Here
                    </a>
                </p>
                <p style='font-size: 14px; color: #868e96;'>If you didn’t request this, you can safely ignore this email.</p>
            </div>
        </body>
        </html>";
        }
    }
}

