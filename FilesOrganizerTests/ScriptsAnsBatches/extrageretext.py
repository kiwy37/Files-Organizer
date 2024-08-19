import json
from docx import Document
from striprtf.striprtf import rtf_to_text
from odf import text, teletype
from odf.opendocument import load
import win32com.client
import pandas as pd
import xml.etree.ElementTree as ET
import pandas as pd
import os
import PyPDF2

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
    return data

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
        data = json.load(json_file)
    return data 

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

def main():
    file_path = r'C:\Users\Kiwy\Desktop\Contul meu Drive\Presentation\document.docx'  

    # Create a dictionary mapping file extensions to extraction functions
    extractors = {
        '.htm': extract_data_from_htm,
        '.html': extract_data_from_html,
        '.xml': extract_data_from_xml,
        '.xlsx': extract_data_from_xlsx,
        '.xls': extract_data_from_xls,
        '.doc': extract_text_from_doc,
        '.odt': extract_text_from_odt,
        '.rtf': extract_text_from_rtf,
        '.docx': extract_text_from_docx,
        '.json': extract_content_from_json,
        '.txt': extract_data_from_txt,
        '.pdf': extract_text_from_pdf
    }

    _, file_extension = os.path.splitext(file_path)

    extract = extractors.get(file_extension)

    if extract is not None:
        content = extract(file_path)
        print(content)
    else:
        print(f'No extractor found for {file_extension} files')

if __name__ == "__main__":
    main()