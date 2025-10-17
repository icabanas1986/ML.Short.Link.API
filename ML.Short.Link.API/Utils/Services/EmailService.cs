using Microsoft.Extensions.Options;
using ML.Short.Link.API.Models;
using ML.Short.Link.API.Utils.Interface;
using System.Net;
using System.Net.Mail;

namespace ML.Short.Link.API.Utils.Services
{
    public class EmailService:IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendRegistrationConfirmationAsync(string toEmail, string userName, string confirmationLink)
        {
            try
            {
                var subject = "Confirma tu registro - Acortador de Links";
                var body = CreateRegistrationConfirmationEmail(userName, confirmationLink);

                await SendEmailAsync(toEmail, subject, body);

                _logger.LogInformation("Email de confirmación enviado a {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email de confirmación a {Email}", toEmail);
                throw;
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };

                message.To.Add(toEmail);

                message.Headers.Add("X-Mailer", "LinkShort");
                message.Headers.Add("X-Priority", "3");


                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {Email}", toEmail);
                throw;
            }
        }

        public string GenerateConfirmationToken()
        {
            // Implementa tu lógica para generar tokens
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }


        private string CreateRegistrationConfirmationEmail(string userName, string confirmationLink)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Confirma tu registro</title>
            <script src='https://cdn.tailwindcss.com'></script>
        </head>
        <body class='bg-gray-50 font-sans'>
            <div class='max-w-2xl mx-auto bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden'>
                <!-- Header -->
                <div class='bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-8 text-center'>
                    <h1 class='text-3xl font-bold text-white mb-2'>¡Bienvenido a LinkShort!</h1>
                    <p class='text-blue-100 text-lg'>Confirma tu dirección de correo electrónico</p>
                </div>

                <!-- Content -->
                <div class='px-6 py-8'>
                    <p class='text-gray-700 text-lg mb-6'>Hola <strong>{WebUtility.HtmlEncode(userName)}</strong>,</p>
                    
                    <p class='text-gray-600 mb-6'>
                        Gracias por registrarte en nuestro acortador de links. Para activar tu cuenta y comenzar a acortar tus enlaces, 
                        por favor confirma tu dirección de correo electrónico haciendo clic en el siguiente botón:
                    </p>

                    <!-- Confirmation Button -->
                    <div class='text-center my-8'>
                        <a href='{WebUtility.HtmlEncode(confirmationLink)}' 
                           class='inline-block bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-8 rounded-lg transition duration-200 transform hover:scale-105 shadow-md'>
                            Confirmar mi cuenta
                        </a>
                    </div>

                    <!-- Alternative Link -->
                    <div class='bg-gray-50 rounded-lg p-4 mb-6'>
                        <p class='text-gray-600 text-sm text-center mb-2'>
                            Si el botón no funciona, copia y pega el siguiente enlace en tu navegador:
                        </p>
                        <p class='text-blue-600 text-sm break-all text-center'>
                            {WebUtility.HtmlEncode(confirmationLink)}
                        </p>
                    </div>

                    <p class='text-gray-600 text-sm mb-4'>
                        Este enlace de confirmación expirará en 24 horas por seguridad.
                    </p>

                    <p class='text-gray-600 text-sm'>
                        Si no te registraste en LinkShort, por favor ignora este mensaje.
                    </p>
                </div>

                <!-- Footer -->
                <div class='bg-gray-50 px-6 py-6 border-t border-gray-200'>
                    <div class='text-center'>
                        <p class='text-gray-500 text-sm mb-2'>
                            &copy; {DateTime.Now.Year} LinkShort. Todos los derechos reservados.
                        </p>
                        <p class='text-gray-400 text-xs'>
                            Este es un mensaje automático, por favor no respondas a este correo.
                        </p>
                    </div>
                </div>
            </div>
        </body>
        </html>";
        }
    }
}
