FROM ubuntu:22.04

# Install dependencies
RUN apt-get update && apt-get install -y \
    curl \
    wget \
    make \
    git \
    libicu-dev \
    && rm -rf /var/lib/apt/lists/*

# Install .NET 9.0 using install-dotnet.sh script
RUN wget https://dot.net/v1/dotnet-install.sh -O install-dotnet.sh \
    && chmod +x install-dotnet.sh \
    && ./install-dotnet.sh --skip-non-versioned-files --channel 9.0 \
    && rm install-dotnet.sh

ENV PATH="$PATH:/root/.dotnet:/root/.dotnet/tools"
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Install Node.js 22
RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y nodejs

# Set working directory
WORKDIR /app

# Copy only configuration files needed for tool restore
COPY . .

# Run entrypoint script
CMD ["./entrypoint.sh"]