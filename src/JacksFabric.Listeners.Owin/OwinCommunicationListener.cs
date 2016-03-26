using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace JacksFabric.WebApi.Stateless
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly IOwinBuilder _owinBuilder;
        private readonly string _appRoot;
        private readonly ServiceInitializationParameters _serviceInitializationParameters;

        private IDisposable _serverHandle;
        private string _listnerAddress;

        public OwinCommunicationListener(string appRoot, IOwinBuilder owinBuilder, ServiceInitializationParameters serviceInitializationParameters)
        {
            _appRoot = appRoot;
            _owinBuilder = owinBuilder;
            _serviceInitializationParameters = serviceInitializationParameters;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var endpoint = _serviceInitializationParameters.CodePackageActivationContext.GetEndpoint("ServiceEndPoint");

            var appRoot = string.IsNullOrWhiteSpace(_appRoot) ? 
                string.Empty : 
                _appRoot.TrimEnd('/') + '/';

            _listnerAddress = $"http://+:{endpoint.Port}/{appRoot}";
            
            _serverHandle = WebApp.Start(_listnerAddress, appBuilder => _owinBuilder.Configuration(appBuilder));

            var publishAddress = _listnerAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
            
            //ServiceEventSource.Current.Message($"Listening on {publishAddress}");

            return Task.FromResult(publishAddress);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
           CloseWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            CloseWebServer();
        }

        private void CloseWebServer()
        {
            if (_serverHandle != null)
            {
                try
                {
                    _serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    
                }
            }
        }
    }
}
