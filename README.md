### 
仅供参考：
using System;
using ABBYY.FineReader.Engine;

class Program
{
    static void Main()
    {
        // 初始化 FineReader Engine
        var engine = new LREngine();
        engine.Startup(new LR_InitializeParams { Modules = LR_ModuleInfo.LR_ALL_MODULES });

        try
        {
            // 1. 创建文档并加载文件
            var document = engine.CreateDocument();
            document.LoadDocument("input.pdf", new LR_LoadDocumentParams { Format = LR_DocFormat.LR_PDF });

            // 2. 修改文档内容（示例：修改第一个文本块的文本）
            var firstPage = document.Pages[0];
            foreach (var block in firstPage.Blocks)
            {
                if (block.BlockType == LR_BlockType.LR_TEXT)
                {
                    // 修改文本内容（示例：在开头添加 "Modified: "）
                    block.Text = "Modified: " + block.Text;
                    break; // 仅修改第一个文本块
                }
            }

            // 3. 保存为 Word 文档
            document.SaveDocument("output.docx", new LR_SaveDocumentParams { Format = LR_DocFormat.LR_WORD });

            Console.WriteLine("文件已成功修改并保存为 output.docx");
        }
        catch (Exception ex)
        {
            Console.WriteLine("发生错误: " + ex.Message);
        }
        finally
        {
            // 关闭 FineReader Engine
            engine.Shutdown();
        }
    }
}
