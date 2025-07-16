// ?2019 ABBYY Development Inc.
// SAMPLES code is property of ABBYY, exclusive rights are reserved. 
//
// DEVELOPER is allowed to incorporate SAMPLES into his own APPLICATION and modify it under 
// the  terms of  License Agreement between  ABBYY and DEVELOPER.


// ABBYY FineReader Engine 12 Sample

// Implementations of FREngine::IFileAdapter and FREngine::IImageSource interfaces.

using System;
using System.Collections.Generic;
using System.IO;

using Sample;

namespace BatchProcessing
{
    public class FileAdapterImpl : FREngine.IFileAdapter
    {
        public FileAdapterImpl(string _fileName)
        {
            fileName = _fileName;
        }

        // FREndine::IFileAdapter methods implementation
        #region IFileAdapter Members

        public string GetFileName()
        {
            return fileName;
        }

        public FREngine.IIntsCollection GetPagesToProcess()
        {
            // Process all pages
            return null;
        }

        public string GetPassword()
        {
            return "";
        }

        #endregion

        private string fileName;
    }

    public class ImageSourceImpl : FREngine.IImageSource
    {
        public ImageSourceImpl(string sourceDir, EngineLoader loader_ )
        {
			loader = loader_;
            string extensionsMask = "bmp|dcx|pcx|png|jpg|jpeg|jp2|jpc|jfif|pdf|tif|tiff|gif|djvu|djv|jb2";
            // Get all files from source folder
            string[] fileNames = Directory.GetFiles(sourceDir, "*.*");
            



            // Select files with appropriate extensions
            List<String> imagesNames = new List<string>();
            foreach (string fileName in fileNames)
            {
                if (extensionsMask.Contains(Path.GetExtension(fileName).Remove(0, 1).ToLower()))
                {
                    imagesNames.Add(fileName);
                }
            }
            // Get enumerator for images
            enumerator = imagesNames.GetEnumerator();
            isEmpty = !enumerator.MoveNext();
        }

        // FREndine::IImageSource methods implementation
        #region IImageSource Members

        public FREngine.IFileAdapter GetNextImageFile()
        {
            if ( isEmpty ) {
                // Return null if there are no more files in source folder
                return null;
            }
            // Create adapter for a current file name
            FileAdapterImpl fileAdapter = new FileAdapterImpl(enumerator.Current);
            isEmpty = !enumerator.MoveNext();
            return fileAdapter;
        }

        public bool IsEmpty()
        {
            return isEmpty;
        }

        // Implement this method only if you need to open images that are already loaded to memory.
        public FREngine.ImageDocument GetNextImageDocument()
        {
            return null;
        }
        #endregion

        List<String>.Enumerator enumerator;
        bool isEmpty;
        EngineLoader loader;
    }
}
