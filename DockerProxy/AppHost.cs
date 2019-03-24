using Funq;
using ServiceStack;
using System.Net;
using System.Collections.Generic;
using ServiceStack.Api.Swagger;
using ServiceStack.Admin;
using ServiceStack.Messaging;
using ServiceStack.Auth;
using System;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;
using ServiceStack.Caching;
using ServiceStack.Validation;

using ServiceStack.Configuration;
using ServiceStack.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Resources;
using ServiceInterface;
using ServiceStack.OrmLite;
using ServiceStack.Data;
using ServiceStack.Web;

namespace DockerProxy
{
    public class AppHost : AppHostBase
    {
        private IConfiguration _configuration { get; }

        private byte[] authKey;
        private byte[] fallbackAuthKey;

        public RSAParameters? JwtRsaPrivateKey;
        public RSAParameters? JwtRsaPublicKey;
        public bool JwtEncryptPayload = false;
        public List<byte[]> FallbackAuthKeys = new List<byte[]>();
        public List<RSAParameters> FallbackPublicKeys = new List<RSAParameters>();

        /// <summary>
        /// Base constructor requires a Name and Assembly where web service implementation is located
        /// </summary>
        public AppHost(IConfiguration configuration)
            : base("Test.Proxy.Host", typeof(TestService).Assembly)
        {
            _configuration = configuration;
            //  Licensing.RegisterLicense(_configuration.GetSection("ServiceStack").GetSection("servicestack:license").Value);
        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        // public override void Configure(Container container)
        // {

        //     authKey = AesUtils.CreateKey();
        //     fallbackAuthKey = AesUtils.CreateKey();

        //     var privateKey = RsaUtils.CreatePrivateKeyParams(RsaKeyLengths.Bit2048);
        //     var publicKey = privateKey.ToPublicRsaParameters();
        //     var privateKeyXml = privateKey.ToPrivateKeyXml();
        //     var publicKeyXml = privateKey.ToPublicKeyXml();

        //     AppSettings = new DictionarySettings(new Dictionary<string, string> {
        //             { "jwt.AuthKeyBase64", Convert.ToBase64String(authKey) },
        //             { "jwt.AuthKeyBase64.1", Convert.ToBase64String(fallbackAuthKey) },
        //         {"PrivateKeyXml" ,"{PrivateKey2016Xml}"},
        //             { "jwt.RequireSecureConnection", "False" },
        //         {"oauth.GoogleOAuth.ConsumerKey","485362488543-e1eiujr63lmf88hd4v5roaq2hrtsfgul.apps.googleusercontent.com" },
        //          {"oauth.GoogleOAuth.ConsumerSecret","ryNIz_W-2KDmQ6E4RPXQcJEM" }
        //         });


        //     SetConfig(new HostConfig
        //     {
        //         DebugMode = true,

        //         UseCamelCase = true,
        //     });
        //     SetConfig(new HostConfig
        //     {
        //         DefaultContentType = MimeTypes.Json
        //     });


        //     //    Plugins.Add(new CorsFeature());
        //     Plugins.Add(new PostmanFeature());
        //     Plugins.Add(new SwaggerFeature());
        //     Plugins.Add(new AdminFeature());
        //     Plugins.Add(new ValidationFeature());
        //     string[] allowOrgins = new string[] {"http://localhost:3002","http://192.168.110.103:3000",
        //         "http://192.168.110.103:3001", "http://192.168.110.103:3002", "http://192.168.110.100:3002",
        //         "http://192.168.110.101:3002", "http://192.168.110.104:3002","http://192.168.110.101","http://192.168.110.101:3000",
        //         "http://192.168.110.104","http://192.168.110.104:3000","http://192.168.110.100:3000","http://192.168.110.100:3000",
        //         "http://192.168.110.105:51581", "http://192.168.13.30:3000","http://localhost:5054", "http://localhost:3000",
        //         "http://localhost:3001", "http://localhost","http://192.168.120.15:3000","http://192.168.120.16:3000",
        //           "https://centerlink.motherflower.com", "https://www.centerlink.motherflower.com","http://192.168.120.16:3002","http://192.168.120.16:4000","http://192.168.201.15:3000",
        //           "http://192.168.110.14:3000","http://104.227.248.181:3002",
        //            "http://www.motherflower.com", "http://motherflower.com", "https://www.motherflower.com", "https://motherflower.com", "http://centerlink.motherflower.com", "http://www.centerlink.motherflower.com",
        //    };


        //     Plugins.Add(new CorsFeature(allowOriginWhitelist: allowOrgins,
        //             allowedHeaders: "Content-Type, Authorization, Session-Id, Origin, X-Requested-With, Accept",
        //         // allowedHeaders: "Content-Type, Authorization, Session-Id, Origin, Accept",
        //         allowCredentials: true, allowedMethods: "GET, POST, PUT, DELETE, OPTIONS")); //Registers global CORS Headers
        //     this.GlobalRequestFilters.Add((httpReq, httpRes, requestDto) =>
        //     {
        //         if (httpReq.Verb == "OPTIONS")
        //             httpRes.EndRequestWithNoContent();   // v 3.9.60 httpExtensions method before httpRes.EndServiceStackRequest();  



        //     });


        //     var dbFactory = new OrmLiteConnectionFactory(
        //"server=.;Database=IdentityDB;user=sa;pwd=123!@#qwe;MultipleActiveResultSets=True", SqlServerDialect.Provider);

        //     container.Register<IDbConnectionFactory>(dbFactory);
        //     container.Register<IAuthRepository>(c =>
        //         new OrmLiteAuthRepository(dbFactory) { UseDistinctRoleTables = true });

        //     //Create UserAuth RDBMS Tables
        //     container.Resolve<IAuthRepository>().InitSchema();

        //     //Also store User Sessions in SQL Server
        //     container.RegisterAs<OrmLiteCacheClient, ICacheClient>();
        //     container.Resolve<ICacheClient>().InitSchema();

        //     // Plugins.Add(new SessionFeature() { });

        //     //Add Support for 
        //     Plugins.Add(new AuthFeature(() => new AuthUserSession(),
        //         new IAuthProvider[] {
        //              new CredentialsAuthProvider(){ PersistSession = true},
        //              new BasicAuthProvider(),
        //     new JwtAuthProvider(AppSettings) {
        //         PersistSession = true,
        //         //HashAlgorithm = "RS256",
        //         //PrivateKeyXml = AppSettings.GetString("PrivateKeyXml")
        //          AuthKey = JwtRsaPrivateKey != null || JwtRsaPublicKey != null ? null : AesUtils.CreateKey(),
        //                 RequireSecureConnection = false,
        //                 HashAlgorithm = JwtRsaPrivateKey != null || JwtRsaPublicKey != null ? "RS256" : "HS256",
        //                 PublicKey = JwtRsaPublicKey,
        //                 PrivateKey = JwtRsaPrivateKey,
        //                 EncryptPayload = JwtEncryptPayload,
        //                 FallbackAuthKeys = FallbackAuthKeys,
        //                 FallbackPublicKeys = FallbackPublicKeys,
        //                // ExpireTokensIn = TimeSpan.FromDays(365),
        //                 ExpireRefreshTokensIn = TimeSpan.FromDays(365), // Refresh Token Expiry
        //                 Provider = "mp",
        //                 SaveExtendedUserInfo = true,
        //                 SetBearerTokenOnAuthenticateResponse = true,
        //                 SessionExpiry = TimeSpan.FromDays(365),
        //                 Issuer="MF",
        //                 ExpireTokensInDays = 20
        //     },
        //     //new ApiKeyAuthProvider(AppSettings),        //Sign-in with API Key
        //     //new CredentialsAuthProvider(),              //Sign-in with UserName/Password credentials
        //    // new BasicAuthProvider(),                    //Sign-in with HTTP Basic Auth
        //     //new DigestAuthProvider(AppSettings),        //Sign-in with HTTP Digest Auth
        //     //new TwitterAuthProvider(AppSettings),       //Sign-in with Twitter
        //     //new FacebookAuthProvider(AppSettings),      //Sign-in with Facebook
        //     //new YahooOpenIdOAuthProvider(AppSettings),  //Sign-in with Yahoo OpenId
        //     //new OpenIdOAuthProvider(AppSettings),       //Sign-in with Custom OpenId
        //     //new GoogleOAuth2Provider(AppSettings),      //Sign-in with Google OAuth2 Provider
        //     //new LinkedInOAuth2Provider(AppSettings),    //Sign-in with LinkedIn OAuth2 Provider
        //     //new GithubAuthProvider(AppSettings),        //Sign-in with GitHub OAuth Provider
        //     //new YandexAuthProvider(AppSettings),        //Sign-in with Yandex OAuth Provider        
        //     //new VkAuthProvider(AppSettings),            //Sign-in with VK.com OAuth Provider 
        //         }));

        //     //        Plugins.Add(new AuthFeature(() => new AuthUserSession(),
        //     //new IAuthProvider[] {
        //     //    new JwtAuthProvider(AppSettings) { AuthKey = AesUtils.CreateKey() },
        //     //    new CredentialsAuthProvider(AppSettings),
        //     //    //...
        //     //}));

        //     Plugins.Add(new RegistrationFeature());
        // }


        public override void Configure(Container container)
        {
            //Store UserAuth in SQL Server
            var dbFactory = new OrmLiteConnectionFactory(
                "server=.;Database=IdentityDB;user=sa;pwd=123!@#qwe;MultipleActiveResultSets=True",
                SqlServerDialect.Provider);

            container.Register<IDbConnectionFactory>(dbFactory);
            container.Register<IAuthRepository>(c =>
                new OrmLiteAuthRepository(dbFactory) { UseDistinctRoleTables = true });

            //Create UserAuth RDBMS Tables
            container.Resolve<IAuthRepository>().InitSchema();

            //Also store User Sessions in SQL Server
            container.RegisterAs<OrmLiteCacheClient, ICacheClient>();
            container.Resolve<ICacheClient>().InitSchema();


            var privateKey = RsaUtils.CreatePrivateKeyParams(RsaKeyLengths.Bit2048);
            var publicKey = privateKey.ToPublicRsaParameters();
            var privateKeyXml = privateKey.ToPrivateKeyXml();
            var publicKeyXml = privateKey.ToPublicKeyXml();

            // just for testing, create a privateKeyXml on every instance
            Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                new IAuthProvider[]
                {
                    new JwtAuthProvider
                    {
                        HashAlgorithm = "RS256",
                        PrivateKeyXml = privateKeyXml,
                        RequireSecureConnection = false,
                    },
                    new CredentialsAuthProvider()
                }));

            this.GlobalRequestFilters.Add((httpReq, httpRes, requestDto) =>
            {
                httpReq.SetSessionId("2MFD2Q706bNlpgG4MdMq");
                //IRequest req = httpReq;
                //req.Items[Keywords.Session] = null;



            });


            Plugins.Add(new RegistrationFeature());

            // uncomment to create a first new user

            //var authRepo = GetAuthRepository();
            //authRepo.CreateUserAuth(new UserAuth
            //{
            //    Id = 1,
            //    UserName = "panahi",
            //    FirstName = "First",
            //    LastName = "Last",
            //    DisplayName = "Display",
            //}, "p@55word");

        }
    }
}
