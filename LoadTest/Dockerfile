    FROM python:3.11-slim

    # Set the working directory
    WORKDIR /app

    # Copy the requirements.txt file into the container
    COPY requirements.txt /app/

    RUN pip install --upgrade pip
    RUN pip install --no-cache-dir -r requirements.txt

    # Copy the rest of your Locust files into the container
    COPY . /app/
    

    # Expose necessary ports
    EXPOSE 8089
    EXPOSE 5557

    # Specify the command to run Locust
    CMD ["locust"]