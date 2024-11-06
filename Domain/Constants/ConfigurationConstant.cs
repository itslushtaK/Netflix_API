using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constants
{
    public static class ConfigurationConstant
    {

        public static string RedisURL => "Redis:URL";
        public static string RedisDatabaseIndex => "Redis:DatabaseIndex";
        public static string RedisExpirationInDays => "Redis:ExpirationInDays";
        public static string AuthenticationAccessTokenExpirationInMinutes => "Authentication:AccessTokenExpirationInMinutes";
        public static string AuthenticationRefreshTokenExpirationInDays => "Authentication:RefreshTokenExpirationInDays";
        public static string AuthenticationSecret => "Authentication:Secret";
        public static string AuthenticationAudience => "Authentication:Audience";
        public static string AuthenticationIssuer => "Authentication:Issuer";
        public static string ApplicationDomainsCustomer => "ApplicationDomains:Customer";
        public static string ApplicationEndPointsConfirmation => "ApplicationEndPoints:Confirmation";
        public static string ApplicationEndPointsResetPassword => "ApplicationEndPoints:ResetPassword";
        public static string SmtpHost => "Smtp:Host";
        public static string SmtpPort => "Smtp:Port";
        public static string SmtpUserName => "Smtp:UserName";
        public static string SmtpPassword => "Smtp:Password";
        public static string SmtpTenantId => "Smtp:TenantId";
        public static string SmtpClientId => "Smtp:ClientId";
        public static string SmtpClientSecret => "Smtp:ClientSecret";
        public static string ReCaptchaSecretKey => "ReCaptcha:SecretKey";
        public static string ReCaptchaURL => "ReCaptcha:URL";
        public static string FTPHost => "FTP:Host";
        public static string FTPUser => "FTP:User";
        public static string FTPPassword => "FTP:Password";
        public static string DatabaseUser => "Database:User";
        public static string DatabasePassword => "Database:Password";
    }
}
