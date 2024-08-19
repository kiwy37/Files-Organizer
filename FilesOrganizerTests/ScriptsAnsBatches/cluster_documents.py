import os
import warnings
import sys
import ast
import base64
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.cluster import DBSCAN
import numpy as np
import json
from docx import Document
from striprtf.striprtf import rtf_to_text
from odf import text, teletype
from odf.opendocument import load
import win32com.client
import pandas as pd
import xml.etree.ElementTree as ET
import PyPDF2

warnings.filterwarnings("ignore")


def extract_data_from_htm(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        content = file.read()
    return content

def extract_data_from_html(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        content = file.read()
    return content

def extract_data_from_xml(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        content = file.read()
    return content

def extract_data_from_xlsx(file_path):
    data = pd.read_excel(file_path)
    return data

def extract_data_from_xls(file_path):
    data = pd.read_excel(file_path)
    content = data.to_string()
    return content

def extract_text_from_doc(file_path):
    word = win32com.client.Dispatch("Word.Application")
    word.visible = False
    wb = word.Documents.Open(file_path)
    doc = word.ActiveDocument
    full_text = []
    for para in doc.Paragraphs:
        full_text.append(para.Range.Text)
    wb.Close()
    word.Quit()
    return '\n'.join(full_text)

def extract_text_from_odt(file_path):
    doc = load(file_path)
    all_text = []
    for paragraph in doc.getElementsByType(text.P):
        all_text.append(teletype.extractText(paragraph))
    return '\n'.join(all_text)

def extract_text_from_rtf(file_path):
    with open(file_path, 'r') as rtf_file:
        rtf_text = rtf_file.read()
    return rtf_to_text(rtf_text)

def extract_text_from_docx(file_path):
    doc = Document(file_path)
    full_text = []
    for para in doc.paragraphs:
        full_text.append(para.text)
    return '\n'.join(full_text)

def extract_content_from_json(file_path):
    with open(file_path, 'r') as json_file:
        content = json_file.read()
    return content

def extract_data_from_txt(file_path):
    with open(file_path, 'r') as file:
        content = file.read()
    return content

def extract_text_from_pdf(file_path):
    pdf_file_obj = open(file_path, 'rb')
    pdf_reader = PyPDF2.PdfReader(pdf_file_obj)
    num_pages = len(pdf_reader.pages)
    content = ""
    for i in range(num_pages):
        page_obj = pdf_reader.pages[i]
        content += page_obj.extract_text()
    pdf_file_obj.close()
    return content

def read_file_content(file_path, file_extension):
    if file_extension == '.htm':
        return extract_data_from_htm(file_path)
    elif file_extension == '.html':
        return extract_data_from_html(file_path)
    elif file_extension == '.xml':
        return extract_data_from_xml(file_path)
    elif file_extension == '.xlsx':
        return extract_data_from_xlsx(file_path)
    elif file_extension == '.xls':
        return extract_data_from_xls(file_path)
    elif file_extension == '.doc':
        return extract_text_from_doc(file_path)
    elif file_extension == '.odt':
        return extract_text_from_odt(file_path)
    elif file_extension == '.rtf':
        return extract_text_from_rtf(file_path)
    elif file_extension == '.docx':
        return extract_text_from_docx(file_path)
    elif file_extension == '.json':
        return extract_content_from_json(file_path)
    elif file_extension == '.txt':
        return extract_data_from_txt(file_path)
    elif file_extension == '.pdf':
        return extract_text_from_pdf(file_path)
    else:
        return ""

if __name__ == "__main__":
    base64_arg = sys.argv[1]
    # Add the missing padding back
    padding = '=' * (4 - len(base64_arg) % 4)
    base64_arg += padding
    similarity_threshold = float(base64.b64decode(base64_arg).decode('utf-8'))

    input_data = ast.literal_eval(base64.b64decode(sys.argv[2]).decode('utf-8'))

    decoded_files = []
    for file in input_data:
        # Decode the file name
        decoded_file_name = base64.b64decode(file[0]).decode('utf-8')

        # Decode the file path and read the file content
        decoded_file_path = base64.b64decode(file[1]).decode('utf-8')
        file_extension = os.path.splitext(decoded_file_path)[1]
        decoded_file_content = read_file_content(decoded_file_path, file_extension)

        decoded_files.append((decoded_file_name, decoded_file_content))

    # Extract texts from decoded files
    texts = [file[1] for file in decoded_files]

    vectorizer = TfidfVectorizer()
    vectors = vectorizer.fit_transform(texts)

    # Apply DBSCAN clustering
    dbscan = DBSCAN(eps=similarity_threshold, min_samples=1, metric='cosine')
    clusters = dbscan.fit_predict(vectors)

    # Print the groups of similar documents
    for i, cluster in enumerate(clusters):
        print(f'{decoded_files[i][0]} : {cluster}')