namespace Evolution.StepStone.Models
{
    using System.Collections.Generic;

    /// <summary>A CV file downloaded from StepStone.</summary>
    public class StepStoneCV
    {
        /// <summary>The filename of the CV in the Zip archive.</summary>
        public string Filename { get; set; }

        /// <summary>The ID of the candidate whose CV this is.</summary>
        public long ID { get; set; }

        /// <summary>The name of the candidate whose CV this is.</summary>
        public string Name { get; set; }

        /// <summary>The extension of the file found.</summary>
        public string Extension { get; set; }

        /// <summary>The MIME type for the CV file.</summary>
        public string Mime { get; set; }

        /// <summary>The bytes of the file.</summary>
        public IEnumerable<byte> File { get; set; }
    }
}