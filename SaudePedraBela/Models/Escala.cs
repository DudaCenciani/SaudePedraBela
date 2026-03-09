public class Escala
{
    public int Id { get; set; }

    public string Categoria { get; set; }

    public string NomeArquivo { get; set; }

    public string CaminhoArquivo { get; set; }

    public DateTime DataUpload { get; set; }
}
public static class CategoriasEscalas
{
    public static List<string> Lista = new List<string>
    {
        "Clínicos e Especialistas",
        "C.S Angelina Santana Schievenin",
        "UBS Dr. Jorge Hirodi Orita",
        "Academia de Saúde (PAPI)",
        "Base Descentralizada do SAMU 192"
    };
}