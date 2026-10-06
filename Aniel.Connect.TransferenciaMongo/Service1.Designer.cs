namespace Aniel.Connect.ExecucaoService
{
    partial class Service1
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.log = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.log)).BeginInit();
            // 
            // log
            // 
            this.log.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.log_EntryWritten);
            // 
            // Service1
            // 
            this.ServiceName = "Service1";
            ((System.ComponentModel.ISupportInitialize)(this.log)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog log;
    }
}
