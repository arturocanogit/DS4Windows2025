using DS4WinWPF.Infraestructura;

Console.WriteLine("DS4Windows XML Cifrador");
Console.WriteLine("Buscando archivos XML en la carpeta actual...");

string currentDirectory = Directory.GetCurrentDirectory();
string[] xmlFiles = Directory.GetFiles(currentDirectory, "*.xml");

if (xmlFiles.Length == 0)
{
    Console.WriteLine("No se encontraron archivos XML en la carpeta actual.");
    return;
}

AESCrypto crypto = new AESCrypto();
int processedFiles = 0;

foreach (string xmlFile in xmlFiles)
{
    try
    {
        string fileName = Path.GetFileNameWithoutExtension(xmlFile);
        string cifFile = Path.Combine(currentDirectory, fileName + ".cif");
        
        string xmlContent = File.ReadAllText(xmlFile);
        string encryptedContent = crypto.Encrypt(xmlContent);
        
        File.WriteAllText(cifFile, encryptedContent);
        
        Console.WriteLine($"Cifrado: {Path.GetFileName(xmlFile)} -> {Path.GetFileName(cifFile)}");
        processedFiles++;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al procesar {Path.GetFileName(xmlFile)}: {ex.Message}");
    }
}

Console.WriteLine($"\nProceso completado. {processedFiles} archivos cifrados.");