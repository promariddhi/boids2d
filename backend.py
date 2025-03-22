from fastapi import FastAPI, UploadFile
from fastapi.responses import FileResponse
from fastapi.middleware.cors import CORSMiddleware

import pandas as pd
import shutil

from web_scraper import process_excel

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"]
)

@app.post("/upload")
async def process_file(file: UploadFile):
    file_path = f"temp.xlsx"
    with open(file_path, "wb") as buffer:
        shutil.copyfileobj(file.file, buffer)
    process_excel(file_path, "student_results.xlsx")

    return {"downloadUrl": "http://127.0.0.1:8000/download"}

@app.get("/download")
async def download_file():
    return FileResponse("student_results.xlsx")

if __name__ == "main":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)