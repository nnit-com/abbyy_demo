// © 2019 ABBYY Development Inc.
// SAMPLES code is property of ABBYY, exclusive rights are reserved. 
//
// DEVELOPER is allowed to incorporate SAMPLES into his own APPLICATION and modify it under 
// the  terms of  License Agreement between  ABBYY and DEVELOPER.


// ABBYY FineReader Engine 12 Sample

// This sample shows how to create batch processor and use it for recognition.

using System;
using System.Windows.Forms;
using System.IO;
using FREngine;

namespace BatchProcessing
{
    public partial class BatchProcessingForm : Form
    {
        private Sample.EngineLoader engineLoader = null;
        private string sourceFolder = Path.Combine(FreConfig.GetSamplesFolder(), "SampleImages\\Test Input");
        private string resultFolder = Path.Combine(FreConfig.GetSamplesFolder(), "SampleImages\\ExportResults" );
        public BatchProcessingForm()
        {
            InitializeComponent();
        }

        private void displayMessage(string text)
        {
            statusLabel.Text = text;
            this.Update();
        }

        private void loadEngine()
        {
            if (engineLoader == null)
            {
                engineLoader = new Sample.EngineLoader();
            }
        }

        private void loadProfile()
        {
            engineLoader.Engine.LoadPredefinedProfile("DocumentArchiving_Accuracy");
        }

        private void unloadEngine()
        {
            if (engineLoader != null)
            {
                engineLoader.Dispose();
                engineLoader = null;
            }
        }

        private void setupFREngine()
        {
            displayMessage("Loading profile...");
            loadProfile();

            engineLoader.Engine.ParentWindow = this.Handle.ToInt64();
            engineLoader.Engine.ApplicationTitle = this.Text;
        }

        private void processWithEngine()
        {
            try
            {
                // Setup FREngine
                setupFREngine();

                // Batch processing
                batchProcessing();
            }
            catch (Exception error)
            {
                MessageBox.Show(this, error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void batchProcessing()
        {
            displayMessage("Batch processing...");
            // Check source folder existence
            if( !Directory.Exists( sourceFolder ) ) {
                throw new Exception( "Cannot find " + sourceFolder );
            }
            // Create result folder if it doesn't exist
            if( !Directory.Exists( resultFolder ) )
            {
                DirectoryInfo newDir = Directory.CreateDirectory( resultFolder );
                if( !newDir.Exists ) {
                    throw new Exception( "Cannot create " + resultFolder );
                }
            }
            // Create CImageSource for accessing to images files in source folder 
            ImageSourceImpl imageSource = new ImageSourceImpl( sourceFolder, engineLoader );
            if( imageSource.IsEmpty() ) {
                throw new Exception( "No images in specified folder." );
            }
            FREngine.IBatchProcessor batchProcessor = engineLoader.Engine.CreateBatchProcessor();

            // Malay english test
            DocumentProcessingParams documentProcessingParams = engineLoader.Engine.CreateDocumentProcessingParams();
            PageProcessingParams pageProcessingParams = documentProcessingParams.PageProcessingParams;
            RecognizerParams recognizerParams = pageProcessingParams.RecognizerParams;
            recognizerParams.SetPredefinedTextLanguage("English");

            // Start batch processor for specified image source
            batchProcessor.Start(imageSource, null, null, null, null);
            // Obtain recognized pages and export them to RTF format
            FREngine.FRPage page = batchProcessor.GetNextProcessedPage();
            int pageNum = 1;
            while( page != null ) {
                // Synthesize page before export
                page.Synthesize(null);
                // Export page to file with the same name and pdf extension
                string resultFilePath = Path.Combine(resultFolder, Path.GetFileName(page.SourceImagePath) +"_page_"+pageNum+ ".pdf");
                page.Export(resultFilePath, FREngine.FileExportFormatEnum.FEF_DOCX, null);
                page = batchProcessor.GetNextProcessedPage();
                pageNum++;
            }
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Load ABBYY FineReader Engine
                displayMessage("Initializing Engine...");
                loadEngine();

                // Process with ABBYY FineReader Engine
                processWithEngine();

                // Unload ABBYY FineReader Engine
                displayMessage("Deinitializing Engine...");
                unloadEngine();
            }
            catch (Exception error)
            {
                MessageBox.Show(this, error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            displayMessage("");
        }
    }
}