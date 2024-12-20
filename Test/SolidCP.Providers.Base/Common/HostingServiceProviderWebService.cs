// Copyright (c) 2016, SolidCP
// SolidCP is distributed under the Creative Commons Share-alike license
// 
// SolidCP is a fork of WebsitePanel:
// Copyright (c) 2015, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;

namespace SolidCP.Providers
{
    public abstract class HostingServiceProviderWebService: IDisposable
    {
        public ServiceProviderSettingsSoapHeader settings = new ServiceProviderSettingsSoapHeader();

        private RemoteServerSettings serverSettings;
        private ServiceProviderSettings providerSettings;

        private IHostingServiceProvider provider;
        protected virtual IHostingServiceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    // try to create provider class
                    Type providerType = Type.GetType(ProviderSettings.ProviderType);
                    try
                    {
                        provider = (IHostingServiceProvider)Activator.CreateInstance(providerType);

                        ((HostingServiceProviderBase)provider).ServerSettings = ServerSettings;
                        ((HostingServiceProviderBase)provider).ProviderSettings = ProviderSettings;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("Can not create '{0}' provider instance with '{1}' type",
                            ProviderSettings.ProviderName, ProviderSettings.ProviderType), ex);
                    }
                }
                return provider;
            }
        }

        public RemoteServerSettings ServerSettings
        {
            get
            {
                if (serverSettings == null)
                {
                    // parse server settings
                    serverSettings = new RemoteServerSettings(settings.Settings);
                }
                return serverSettings;
            }
            set => serverSettings = value;
        }

        public ServiceProviderSettings ProviderSettings
        {
            get
            {
                if (providerSettings == null)
                {
                    // parse provider settings
                    providerSettings = new ServiceProviderSettings(settings.Settings);
                }
                return providerSettings;
            }
            set { providerSettings = value; }
        }

        public void Dispose()
        {
            if (Provider is IDisposable disposableProvider) disposableProvider.Dispose();    
        }
    }
}
