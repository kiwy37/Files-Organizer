using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOrganizer.Models;

public class Settings : Core.ViewModel, INotifyPropertyChanged
{
    private bool? _textFilesCode = true;
    private bool? _textFilesLanguage = true;
    private bool? _audiosLanguage = true;
    private bool? _imagesCode = true;
    private bool? _videosCode = true;
    private bool? _imagesLanguage = true;
    private bool? _videosLanguage = true;
    private bool? _filterType = false;
    private int _similarityThreshold = 0;

    public Settings()
    {
        ImagesCode = false;
        VideosCode = false;
        ImagesLanguage = false;
        VideosLanguage = false;
        FilterType = false;
        TextFilesCode = false;
        TextFilesLanguage = false;
        AudiosLanguage = false;
        SimilarityThreshold = 0;
    }
    public Settings(List<bool> settings, int similarityThreshold)
    {
        ImagesCode = settings[0];
        VideosCode = settings[1];
        ImagesLanguage = settings[2];
        VideosLanguage = settings[3];
        FilterType = settings[4];
        TextFilesCode = settings[5];
        TextFilesLanguage = settings[6];
        AudiosLanguage = settings[7];
        SimilarityThreshold = similarityThreshold;
    }
    public Settings(Settings settings)
    {
        ImagesCode = settings.ImagesCode;
        VideosCode = settings.VideosCode;
        ImagesLanguage = settings.ImagesLanguage;
        VideosLanguage = settings.VideosLanguage;
        FilterType = settings.FilterType;
        TextFilesCode = settings.TextFilesCode;
        TextFilesLanguage = settings.TextFilesLanguage;
        AudiosLanguage = settings.AudiosLanguage;
        SimilarityThreshold = settings.SimilarityThreshold;
    }
    //public Settings(Tuple<bool, bool, bool, bool, bool, bool, bool, bool> tuple)
    //{
    //    ImagesCode = tuple.Item1;
    //    VideosCode = tuple.Item2;
    //    ImagesLanguage = tuple.Item3;
    //    VideosLanguage = tuple.Item4;
    //    FilterType = tuple.Item5;

    //    TextFilesCode = tuple.Item6;
    //    TextFilesLanguage = tuple.Item7;
    //    AudiosLanguage = tuple.Rest;
    //}
    
    public int SimilarityThreshold
    {
        get { return _similarityThreshold; }
        set
        {
            if (_similarityThreshold != value)
            {
                _similarityThreshold = value;
                OnPropertyChanged(nameof(SimilarityThreshold));
            }
        }
    }
    public bool? TextFilesCode
    {
        get { return (_textFilesCode != null) ? _textFilesCode : false; }
        set
        {
            _textFilesCode = value;
            OnPropertyChanged(nameof(TextFilesCode));
        }
    }

    public bool? TextFilesLanguage
    {
        get { return (_textFilesLanguage != null) ? _textFilesLanguage : false; }
        set
        {
            _textFilesLanguage = value;
            OnPropertyChanged(nameof(TextFilesLanguage));
        }
    }

    public bool? AudiosLanguage
    {
        get { return (_audiosLanguage != null) ? _audiosLanguage : false; }
        set
        {
            _audiosLanguage = value;
            OnPropertyChanged(nameof(AudiosLanguage));
        }
    }

    public bool? ImagesCode
    {
        get { return (_imagesCode != null) ? _imagesCode : false; }
        set
        {
            _imagesCode = value;
            OnPropertyChanged(nameof(ImagesCode));
        }
    }

    public bool? VideosCode
    {
        get { return (_videosCode != null) ? _videosCode : false; }
        set
        {
            _videosCode = value;
            OnPropertyChanged(nameof(VideosCode));
        }
    }

    public bool? ImagesLanguage
    {
        get { return (_imagesLanguage != null) ? _imagesLanguage : false; }
        set
        {
            _imagesLanguage = value;
            OnPropertyChanged(nameof(ImagesLanguage));
        }
    }

    public bool? VideosLanguage
    {
        get { return (_videosLanguage != null) ? _videosLanguage : false; }
        set
        {
            _videosLanguage = value;
            OnPropertyChanged(nameof(VideosLanguage));
        }
    }

    public bool? FilterType
    {
        get { return (_filterType != null) ? _filterType : false; }
        set
        {
            _filterType = value;
            OnPropertyChanged(nameof(FilterType));
        }
    }
}
