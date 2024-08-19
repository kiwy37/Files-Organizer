import os
import sys
import base64
import ast
from skimage import io 
from skimage.metrics import structural_similarity as compare_ssim
import numpy as np

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
        print(decoded_file_name)
        # Decode the file path and read the file content
        decoded_file_path = base64.b64decode(file[1]).decode('utf-8')
        print(decoded_file_path)
        decoded_image = io.imread(decoded_file_path, as_gray=True)

        decoded_files.append((decoded_file_name, decoded_image))

    

    # Calculate SSIM for each pair of images
    ssim_values = []
    for i in range(len(decoded_files)):
        for j in range(i+1, len(decoded_files)):
            ssim = compare_ssim(decoded_files[i][1], decoded_files[j][1])
            ssim_values.append((decoded_files[i][0], decoded_files[j][0], ssim))

    # Print the pairs of similar images
    for file1, file2, ssim in ssim_values:
        if ssim > similarity_threshold:
            print(f'{file1} and {file2} are similar with SSIM: {ssim}')

    print(f'Total number of image pairs: {len(ssim_values)}')



    # if __name__ == "__main__":
#     # Hardcoded values for testing
#     similarity_threshold = 0.2  # replace with your desired threshold
#     input_data = [("floare3.jpg", r"C:\Users\Kiwy\Desktop\Presentation\Coding things\Pics differences\floare3.jpg"), ("floare3 blur.jpg", r"C:\Users\Kiwy\Desktop\Presentation\Coding things\Pics differences\floare3 blur.jpg")]
#     decoded_files = []
#     for file in input_data:
#         # Use the file name directly
#         decoded_file_name = file[0]

#         # Use the file path directly and read the file content
#         decoded_file_path = file[1]
#         decoded_image = io.imread(decoded_file_path, as_gray=True)

#         decoded_files.append((decoded_file_name, decoded_image))

#     # Calculate SSIM for each pair of images
#     ssim_values = []
#     for i in range(len(decoded_files)):
#         for j in range(i+1, len(decoded_files)):
#             try:
#                 print(f'Comparing image {i} with image {j}')
#                 ssim = compare_ssim(decoded_files[i][1], decoded_files[j][1], data_range=1.0)
#                 ssim_values.append((decoded_files[i][0], decoded_files[j][0], ssim))
#             except Exception as e:
#                 print(f'Error comparing image {i} and image {j}: {e}')

#     # Print the pairs of similar images
#     for file1, file2, ssim in ssim_values:
#         try:
#             if ssim > similarity_threshold:
#                 print(f'{file1} and {file2} are similar with SSIM: {ssim}')
#         except Exception as e:
#             print(f'Error printing similarity for {file1} and {file2}: {e}')

#     try:
#         print(f'Total number of image pairs: {len(ssim_values)}')
#     except Exception as e:
#         print(f'Error printing total number of image pairs: {e}')