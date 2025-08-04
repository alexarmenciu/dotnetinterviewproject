## Dockerizing the Application

### 1. Build the Docker image

```sh
docker build -t dotnetinterviewproject .
```

### 2. Run the Docker container

```sh
docker run -it --rm -p 5000:5000 -p 5001:5001 dotnetinterviewproject
```

- This will expose the API on port 5000 and the Blazor frontend on port 5001.
- The container will automatically start both services with proper port configuration.

### 3. Access your application

- API: [http://localhost:5000](http://localhost:5000)
- Frontend: [http://localhost:5001/tasks](http://localhost:5001/tasks)

---

**Note:**

- If you need to pass environment variables, use `-e VAR=value` with `docker run`.
- The Dockerfile is set up to run both the API and the frontend together with separate port configurations.
- If you encounter port conflicts, ensure no other services are running on ports 5000 or 5001.
