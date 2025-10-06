using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ArtificialSoul.Memory
{
    public class SessionMemory
    {
        private List<DialogueEntry> memory = new List<DialogueEntry>();
        private string sessionId;
        private string memoryFilePath;
        private string memoryCfgPath = "memory";

        public SessionMemory(string sessionId = null)
        {
            this.sessionId = sessionId ?? Guid.NewGuid().ToString();
            if(!Path.Exists(memoryCfgPath))
                Directory.CreateDirectory(memoryCfgPath);
            this.memoryFilePath =Path.Combine(memoryCfgPath,$"memory_{this.sessionId}.json");
            LoadMemory();
        }

        public void AddEntry(string userInput, string modelAOutput, string modelBOutput, string modelCOutput)
        {
            var entry = new DialogueEntry
            {
                Timestamp = DateTime.UtcNow,
                UserInput = userInput,
                ModelAOutput = modelAOutput,
                ModelBOutput = modelBOutput,
                ModelCOutput = modelCOutput
            };

            memory.Add(entry);
            SaveMemory();
        }

        public List<DialogueEntry> GetRecentHistory(int count)
        {
            int startIndex = Math.Max(0, memory.Count - count);
            return memory.GetRange(startIndex, Math.Min(count, memory.Count - startIndex));
        }

        public string SessionId => sessionId;

        private void LoadMemory()
        {
            if (File.Exists(memoryFilePath))
            {
                try
                {
                    string json = File.ReadAllText(memoryFilePath);
                    memory = JsonConvert.DeserializeObject<List<DialogueEntry>>(json) ?? new List<DialogueEntry>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading memory: {ex.Message}");
                }
            }
        }

        private void SaveMemory()
        {
            try
            {
                string json = JsonConvert.SerializeObject(memory, Formatting.Indented);
                File.WriteAllText(memoryFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving memory: {ex.Message}");
            }
        }
    }

    public class DialogueEntry
    {
        public DateTime Timestamp { get; set; }
        public string UserInput { get; set; }
        public string ModelAOutput { get; set; }
        public string ModelBOutput { get; set; }
        public string ModelCOutput { get; set; }
    }
}