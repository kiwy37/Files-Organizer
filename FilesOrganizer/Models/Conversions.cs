using System.Collections.ObjectModel;

namespace FilesOrganizer.Models;

public class MyCheckbox
{
    public bool IsChecked { get; set;}
    public string Content { get; set; }

    public MyCheckbox(string content, bool isChecked)
    {
        Content = content;
        IsChecked = isChecked;
    }
}
public static class Conversions
{
    //  DOCX to PDF done
    //  DOCX to TXT done

    //  PDF to DOCX done
    //  PDF to TXT          nu merge   !!!!!!!!!

    //  TXT to PDF done
    //  TXT to DOCX done

    //  TXT to ODT

    public static ObservableCollection<MyCheckbox> Texts { get; set; } = new ObservableCollection<MyCheckbox>()
    {
        new MyCheckbox("TXT", false),               
        new MyCheckbox("PDF", false),
        new MyCheckbox("RTF", false),
        new MyCheckbox("ODT", false),
        new MyCheckbox("DOC", false),
        new MyCheckbox("DOCX", false),
    };

    public static ObservableCollection<MyCheckbox> Images { get; set; } = new ObservableCollection<MyCheckbox>()
    {
        new MyCheckbox("JPG", false),
        new MyCheckbox("PNG", false),
        new MyCheckbox("GIF", false),
    };

    public static ObservableCollection<MyCheckbox> Videos { get; set; } = new ObservableCollection<MyCheckbox>()
    {
        new MyCheckbox("MP4", false),
        new MyCheckbox("AVI", false),
        new MyCheckbox("MKV", false),
    };

    public static ObservableCollection<MyCheckbox> Audios { get; set; } = new ObservableCollection<MyCheckbox>()
    {
        new MyCheckbox("MP3", false),
        new MyCheckbox("WAV", false),
        new MyCheckbox("WMA", false),
    };

    public static ObservableCollection<MyCheckbox> Unknown { get; set; } = new ObservableCollection<MyCheckbox>()
    {
    };
}