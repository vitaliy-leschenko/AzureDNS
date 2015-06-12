using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Unity;

namespace AzureDNS.Core.PS
{
    public class PowerShellUI : PSHostUserInterface
    {
        private readonly PowerShellRawUI rawUi;
        private readonly ILoggerFacade logger;

        public PowerShellUI(IUnityContainer container)
        {
            rawUi = container.Resolve<PowerShellRawUI>();
            logger = container.Resolve<ILoggerFacade>();
        }

        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        public override SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }

        public override void Write(string value)
        {
            logger.Log(value, Category.Info, Priority.None);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            logger.Log(value, Category.Info, Priority.None);
        }

        public override void WriteLine(string value)
        {
            logger.Log(value, Category.Info, Priority.None);
        }

        public override void WriteErrorLine(string value)
        {
            logger.Log(value, Category.Exception, Priority.None);
        }

        public override void WriteDebugLine(string message)
        {
            logger.Log(message, Category.Debug, Priority.None);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            throw new NotImplementedException();
        }

        public override void WriteVerboseLine(string message)
        {
            logger.Log(message, Category.Info, Priority.None);
        }

        public override void WriteWarningLine(string message)
        {
            logger.Log(message, Category.Warn, Priority.None);
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName,
            PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return rawUi; }
        }
    }
}