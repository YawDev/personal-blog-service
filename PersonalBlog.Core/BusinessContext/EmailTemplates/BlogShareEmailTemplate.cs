using System.Net;

namespace PersonalBlog.Core.BusinessContext.EmailTemplates
{
    /// <summary>
    /// Builds the "an article was shared with you" email. Returns both an HTML body
    /// (table-based, inline styles for broad email-client support) and a plain-text
    /// fallback for clients that don't render HTML.
    /// </summary>
    public static class BlogShareEmailTemplate
    {
        private const string BlogName = "Personal Blog";
        // Matches the frontend's primary accent (Tailwind teal-600 = #0d9488; hover/darker is teal-700 = #0f766e).
        private const string AccentColor = "#0d9488";

        public static (string Subject, string HtmlBody, string PlainTextBody) Build(string shareLink, string? sharedByName = null)
        {
            // Encode anything that lands in HTML text/attributes to avoid breaking the markup.
            var safeLink = WebUtility.HtmlEncode(shareLink);
            var sharer = string.IsNullOrWhiteSpace(sharedByName) ? "Someone" : WebUtility.HtmlEncode(sharedByName);

            var subject = $"{(string.IsNullOrWhiteSpace(sharedByName) ? "Someone" : sharedByName)} shared an article with you";

            var htmlBody = $@"<!DOCTYPE html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <meta name=""x-apple-disable-message-reformatting"" />
  <title>{BlogName}</title>
</head>
<body style=""margin:0; padding:0; background-color:#f4f4f7; font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif;"">
  <!-- preheader: hidden preview text shown in the inbox list -->
  <div style=""display:none; max-height:0; overflow:hidden; opacity:0;"">
    {sharer} thought you'd enjoy this read on {BlogName}.
  </div>

  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f4f7;"">
    <tr>
      <td align=""center"" style=""padding:32px 16px;"">

        <table role=""presentation"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px; width:100%; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 1px 3px rgba(0,0,0,0.08);"">

          <!-- header -->
          <tr>
            <td style=""background-color:{AccentColor}; padding:24px 32px;"">
              <span style=""color:#ffffff; font-size:20px; font-weight:700; letter-spacing:0.3px;"">{BlogName}</span>
            </td>
          </tr>

          <!-- body -->
          <tr>
            <td style=""padding:40px 32px 8px 32px;"">
              <h1 style=""margin:0 0 16px 0; font-size:24px; line-height:1.3; color:#111827;"">
                {sharer} shared an article with you
              </h1>
              <p style=""margin:0 0 28px 0; font-size:16px; line-height:1.6; color:#4b5563;"">
                They thought this one was worth your time. Give it a read whenever you have a moment &mdash; no account needed.
              </p>
            </td>
          </tr>

          <!-- CTA button -->
          <tr>
            <td align=""center"" style=""padding:0 32px 32px 32px;"">
              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"" style=""border-radius:8px; background-color:{AccentColor};"">
                    <a href=""{safeLink}"" target=""_blank""
                       style=""display:inline-block; padding:14px 32px; font-size:16px; font-weight:600; color:#ffffff; text-decoration:none; border-radius:8px;"">
                      Read the article &rarr;
                    </a>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- fallback link -->
          <tr>
            <td style=""padding:0 32px 40px 32px;"">
              <p style=""margin:0; font-size:13px; line-height:1.5; color:#9ca3af;"">
                If the button doesn't work, copy and paste this link into your browser:<br />
                <a href=""{safeLink}"" target=""_blank"" style=""color:{AccentColor}; word-break:break-all;"">{safeLink}</a>
              </p>
            </td>
          </tr>

          <!-- footer -->
          <tr>
            <td style=""background-color:#f9fafb; padding:24px 32px; border-top:1px solid #eef0f4;"">
              <p style=""margin:0; font-size:12px; line-height:1.5; color:#9ca3af;"">
                You received this email because someone shared an article from {BlogName} with you.
                You can safely ignore it if it wasn't meant for you.
              </p>
            </td>
          </tr>

        </table>

      </td>
    </tr>
  </table>
</body>
</html>";

            var plainTextBody =
$@"{(string.IsNullOrWhiteSpace(sharedByName) ? "Someone" : sharedByName)} shared an article with you on {BlogName}.

Read it here:
{shareLink}

You received this email because someone shared an article from {BlogName} with you. You can safely ignore it if it wasn't meant for you.";

            return (subject, htmlBody, plainTextBody);
        }
    }
}
