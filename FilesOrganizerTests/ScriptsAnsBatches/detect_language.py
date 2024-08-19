import subprocess
import sys
from pydub import AudioSegment
from moviepy.editor import VideoFileClip
from langdetect import detect
import base64
import whisper

def extract_audio_from_video(video_file):
    video = VideoFileClip(video_file)
    trimmed_video = video.subclip(0, 30) 
    trimmed_video.audio.write_audiofile("extracted_audio.wav")
    return "extracted_audio.wav"

def detect_language_text_file(file):
    with open(file, 'r') as f:
        text = f.read()
    try:
        return detect(text)
    except:
    #   return "Language detection failed"
        return "None"

def detect_language(file, case):
    language = "Unsupported file format"
    # if file.endswith(('.mp4', '.avi', '.mov', '.mkv', '.flv', '.wmv', '.webm', '.mpeg', '.mpg', '.3gp', '.mp3', '.wav')): 
    if case == 'a':
        if file.endswith(('.mp3', '.wav')): 
            audio_file = file
            audio = whisper.load_audio(audio_file)
            audio = whisper.pad_or_trim(audio)
            model = whisper.load_model("base")
            mel = whisper.log_mel_spectrogram(audio).to(model.device)
            _, probs = model.detect_language(mel)
            language = max(probs, key=probs.get)
        else:
            audio_file = extract_audio_from_video(file)
            audio = whisper.load_audio(audio_file)
            audio = whisper.pad_or_trim(audio)
            model = whisper.load_model("base")
            mel = whisper.log_mel_spectrogram(audio).to(model.device)
            _, probs = model.detect_language(mel)
            language = max(probs, key=probs.get)
        return language
    else:
        language =  detect(file)
    return language
          
if __name__ =="__main__":
    file = base64.b64decode(sys.argv[1]).decode('utf-8')
    case = sys.argv[2]                                  # a - audio, t - text
    print(detect_language(file,case))