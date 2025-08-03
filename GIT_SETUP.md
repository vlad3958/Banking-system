# Git Setup Script for Banking System

This script will help you set up your Git repository and make your first commit.

## If you need to initialize a new repository:

```powershell
# Navigate to your project directory
cd "C:\Users\Влад\source\repos\Banking"

# Initialize Git repository
git init

# Add remote origin (replace with your actual repository URL)
git remote add origin https://github.com/vlad3958/Banking-system.git
```

## If you already have a repository but lost the .git folder:

```powershell
# Clone your existing repository to a temporary location
git clone https://github.com/vlad3958/Banking-system.git temp-banking

# Copy the .git folder to your current project
xcopy "temp-banking\.git" ".git" /E /I /H

# Remove the temporary folder
rmdir /S temp-banking

# Check status
git status
```

## Add and commit your current changes:

```powershell
# Add all files
git add .

# Make your first commit
git commit -m "Initial commit: Banking system with unit tests and documentation"

# Push to remote repository
git push -u origin main
```

## If you get authentication errors:

1. **Using HTTPS**: You'll need a Personal Access Token
   - Go to GitHub Settings > Developer settings > Personal access tokens
   - Create a new token with repo permissions
   - Use the token as your password when prompted

2. **Using SSH**: Set up SSH keys
   ```powershell
   # Generate SSH key (if you don't have one)
   ssh-keygen -t rsa -b 4096 -C "your-email@example.com"
   
   # Add SSH key to GitHub
   # Copy the content of ~/.ssh/id_rsa.pub and add it to GitHub
   ```

## Common Git commands for future use:

```powershell
# Check status
git status

# Add specific files
git add filename.cs

# Add all changes
git add .

# Commit changes
git commit -m "Your commit message"

# Push changes
git push

# Pull latest changes
git pull

# Create new branch
git checkout -b feature/new-feature

# Switch between branches
git checkout main
git checkout feature/new-feature

# Merge branch
git checkout main
git merge feature/new-feature
```

## Repository Structure

Your repository should include:
- ✅ Source code (Banking/, Banking.DAL/, Banking.Tests/)
- ✅ Documentation (README.md, QUICKSTART.md, API_REFERENCE.md, DEVELOPMENT.md)
- ✅ Configuration files (.gitignore, *.sln, *.csproj)
- ❌ Build outputs (bin/, obj/ - excluded by .gitignore)
- ❌ User-specific files (.vs/, *.user - excluded by .gitignore)
