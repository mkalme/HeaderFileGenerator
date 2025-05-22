namespace HeaderFileGenerator
{
    public struct Configuration 
    {
        public string Solution { get; set; }
        public string SourceFolder { get; set; }
        public string[] Projects { get; set; }
        public string OutputFolder { get; set; }
    }
}
