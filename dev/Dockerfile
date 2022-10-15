FROM mcr.microsoft.com/dotnet/sdk:6.0

# Install missing 'ps' command that some VS Code extensions depend on:
RUN apt-get update && \
    apt-get install -y procps

RUN groupadd -g 1000 -r core && \
    useradd -u 1000 -r -g core -m -s $(which bash) core
# Option '-m' to create home directory (see https://askubuntu.com/a/393470)
# Option '-s' to set  shell for this user (see comment in https://askubuntu.com/a/393470)
# Option '-r' creates a system user which does not expire 
#             (see https://linuxize.com/post/how-to-create-users-in-linux-using-the-useradd-command/)

WORKDIR /app

# Install dotnet-outdated (see https://github.com/dotnet-outdated/dotnet-outdated)
RUN runuser -l core -c "dotnet tool install --global dotnet-outdated-tool"
# runuser installs it as if the non-root user was installing it. This makes 
# it available to that non-root user


ENV PATH "$PATH:/home/core/.dotnet/tools"
USER core
