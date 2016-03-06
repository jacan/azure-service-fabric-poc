using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;

using static System.FormattableString;

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

            var port = endpoint.Port;
            var appRoot = string.IsNullOrWhiteSpace(_appRoot) ? 
                string.Empty : 
                _appRoot.TrimEnd('/') + '/';

            _listnerAddress = Invariant($"http://+:{endpoint.Port}/{appRoot}");

            //_listnerAddress = string.Format(
            //    CultureInfo.InvariantCulture,
            //    "http://+:{0}/{1}",
            //    port,
            //    string.IsNullOrWhiteSpace(_appRoot) ? string.Empty : _appRoot.TrimEnd('/') + '/' 
            //    );

            _serverHandle = WebApp.Start(_listnerAddress, appBuilder => _owinBuilder.Configuration(appBuilder));

            var publishAddress = _listnerAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
            
            ServiceEventSource.Current.Message($"Listening on {publishAddress}");

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
