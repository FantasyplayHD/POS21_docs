using System.Text.RegularExpressions;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("---------------------------------------------------------");
Console.WriteLine("|                                                       |");
Console.WriteLine("|  Bitte geben Sie den gewünschten Pfad zum renamen an  |");
Console.WriteLine("|                                                       |");
Console.WriteLine("---------------------------------------------------------");
Console.Write("> ");
string directory = Console.ReadLine();
Console.WriteLine();
if (Directory.Exists(directory)) {
    string[] subdirectoryEntries = Directory.GetDirectories(directory);
    foreach (string subdirectory in subdirectoryEntries) {
        if (Directory.Exists(subdirectory)) {
            string[] filesinSub = Directory.GetFiles(subdirectory);
            foreach (string fileName in filesinSub) {
                if (fileName.Contains("═")) {
                    //Console.WriteLine(fileName);
                    string[] fileNameParts = fileName.Split('\\');
                    string myString = Regex.Replace(fileNameParts[fileNameParts.Length - 1], @"\s+", " ");
                    string[] allParts = myString.Split(' ');
                    allParts[1] = "_";
                    string newPath = allParts[0] + allParts[1] + allParts[2];
                    for (int i = 3; i < allParts.Length; i++) {
                        newPath += $" {allParts[i]} ";
                    }
                    newPath = Regex.Replace(newPath, @"\s+", " ");
                    string newerPath = ($"{subdirectory}\\{newPath}");
                    System.IO.File.Move(fileName, newerPath);
                }
            }
        }
    }
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("All done - Press to exit ...");
    Console.ReadKey();
    Environment.Exit(0);

}



