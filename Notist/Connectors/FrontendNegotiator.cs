using Microsoft.Web.WebView2.Core;

namespace Notist.Connectors
{
    internal class FrontendNegotiator
	{
		private CoreWebView2 webView;

		public FrontendNegotiator(CoreWebView2 webView2)
		{
			this.webView = webView2;
			InitHostObjects();
		}

		private void InitHostObjects()
		{
			this.webView.AddHostObjectToScript("app_connector", new AppConnector());
			this.webView.AddHostObjectToScript("notist_connector", new NotistConnector());
			this.webView.AddHostObjectToScript("settings_connector", new SettingsConnector());
		}
    }
}
