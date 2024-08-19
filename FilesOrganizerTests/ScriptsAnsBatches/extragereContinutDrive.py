import os.path
import io
from google.auth.transport.requests import Request
from google.oauth2.credentials import Credentials
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from googleapiclient.http import MediaIoBaseDownload
from odf.text import P
from odf.opendocument import load
from googleapiclient.http import MediaIoBaseDownload
import PyPDF2

def print_pdf_content(service, file_id):
    request = service.files().get_media(fileId=file_id)
    fh = io.BytesIO()
    downloader = MediaIoBaseDownload(fh, request)
    done = False
    while done is False:
        status, done = downloader.next_chunk()
        print("Download %d%%." % int(status.progress() * 100))

    fh.seek(0)

    pdf_reader = PyPDF2.PdfFileReader(fh)
    total_pages = pdf_reader.getNumPages()

    for page_num in range(total_pages):
        page = pdf_reader.getPage(page_num)
        print(page.extractText())
  
# If modifying these scopes, delete the file token.json.
SCOPES = ["https://www.googleapis.com/auth/drive.readonly"]

def main():
  """Shows basic usage of the Drive v3 API.
  Prints the names and ids of the first 10 files the user has access to.
  """
  creds = None
  # The file token.json stores the user's access and refresh tokens, and is
  # created automatically when the authorization flow completes for the first
  # time.
  if os.path.exists("token.json"):
    creds = Credentials.from_authorized_user_file("token.json", SCOPES)
  # If there are no (valid) credentials available, let the user log in.
  if not creds or not creds.valid:
    flow = InstalledAppFlow.from_client_secrets_file(
        r"C:\Users\Kiwy\Desktop\FilesOrganizer\FilesOrganizer\Credentials\client_secret.json", SCOPES
    )
    creds = flow.run_local_server(port=0)
    # Save the credentials for the next run
    with open("token.json", "w") as token:
      token.write(creds.to_json())

  try:
    service = build("drive", "v3", credentials=creds)
    # Call the Drive v3 API
    results = (
        service.files()
        .list(
            pageSize=10, 
            fields="nextPageToken, files(id, name)",
            q="'me' in owners and trashed = false"
        )
        .execute()
    )
    items = results.get("files", [])

    print_pdf_content(service, "1SYPeXwYtiEQ5Fg2pHe8raWxkbbHmnXxN")

    if not items:
      print("No files found.")
      return
    print("Files:")
    for item in items:
      print(f"{item['name']} ({item['id']})")
  except HttpError as error:
    # TODO(developer) - Handle errors from drive API.
    print(f"An error occurred: {error}")

if __name__ == "__main__":
  main()