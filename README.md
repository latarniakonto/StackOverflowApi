# StackOverflow API

## Users

| Username          | Password      |
| ----------------- | ------------- |
| user@tag.com      | Password@1234?|
| admin.@tag.com    | Password@1234?|

## Running the Application

The application runs on the following address: `http://0.0.0.0:8080`


### Setup Instructions

To set up and run the application, follow these steps:

1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Ensure you have Docker installed on your machine.
4. Run the following command to build and start the Docker container: `docker compose up`

## Tags

1. Login as `user@tag.com` or register a new user.
2. Navigate to the project Tags page.

Pagination and sorting is provided via DataTables aka Javascript tables library.

## OpenAPI

1. Login as `admin@tag.com`
2. Navigate to the Swashbuckle page via dropdown or directly under `http://0.0.0.0:8080/swagger/index.html`
