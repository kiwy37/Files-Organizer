# MIT License

# Copyright (c) 2022 OpenAI

# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

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
    if case == 'a':
        if file.endswith(('.mp3', '.wav')):
            audio_file = file
        else:
            audio_file = extract_audio_from_video(file)
        # load audio and pad/trim it to fit 30 seconds
        audio = whisper.load_audio(audio_file)
        audio = whisper.pad_or_trim(audio)
        model = whisper.load_model("base")
        # make log-Mel spectrogram and move to the same device as the model            
        mel = whisper.log_mel_spectrogram(audio).to(model.device)
        # # detect the spoken language
        _, probs = model.detect_language(mel)
        language = max(probs, key=probs.get)
    else:
        language = detect(file)
    return language
          
if __name__ =="__main__":
    file = base64.b64decode(sys.argv[1]).decode('utf-8')
    case = sys.argv[2]                                  # a - audio, t - text
    print(detect_language(file,case))