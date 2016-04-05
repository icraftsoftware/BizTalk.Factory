namespace Be.Stateless.BizTalk.ClaimStore.Agent
{
	partial class Installer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Installer));
			this._serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this._serviceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// serviceProcessInstaller
			// 
			this._serviceProcessInstaller.Password = null;
			this._serviceProcessInstaller.Username = null;
			// 
			// serviceInstaller
			// 
			this._serviceInstaller.Description = resources.GetString("serviceInstaller.Description");
			this._serviceInstaller.DisplayName = "BizTalk Factory Claim Store Agent Service";
			this._serviceInstaller.ServiceName = "BTF_CSA";
			this._serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// Installer
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
				this._serviceProcessInstaller,
				this._serviceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller _serviceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller _serviceInstaller;
	}
}