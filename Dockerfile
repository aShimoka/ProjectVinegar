# --- MONO INSTALLATION ---
# Get the debian 9 base image.
FROM debian:stretch AS MonoDebian

# First, install mono dependencies.
RUN apt update
RUN apt install apt-transport-https dirmngr -y
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo "deb https://download.mono-project.com/repo/debian/ stretch/snapshots/5.12.0 main" | tee /etc/apt/sources.list.d/mono-official-stable.list

# Install mono compiler.
RUN apt update
RUN apt install mono-complete -y

# --- Godot Download ---
# Get the newly created "MonoDebian" image.
#FROM MonoDebian AS GodotDownloader

# Install curl and unzip.
RUN apt install curl unzip -y

# Fetch godot editor and templates.
WORKDIR /tmp/godot
RUN curl https://codeload.github.com/godotengine/godot/zip/3.0.6-stable -o GodotSources.zip
RUN curl https://downloads.tuxfamily.org/godotengine/3.0.6/mono/Godot_v3.0.6-stable_mono_export_templates.tpz -o GodotTemplates.zip

# Unzip the files.
RUN unzip GodotSources.zip -d /tmp/godot/_unzipped
RUN mkdir -p /tmp/godot/sources \
        && cp -Rp /tmp/godot/_unzipped/*/* /tmp/godot/sources/ \
        && rm -r /tmp/godot/_unzipped
RUN unzip GodotTemplates.zip -d /tmp/godot/_unzipped
RUN mkdir -p /tmp/godot/templates/3.0.6.stable.mono \
        && cp -Rp /tmp/godot/_unzipped/*/* /tmp/godot/templates/3.0.6.stable.mono \
        && rm -r /tmp/godot/_unzipped

# Cleanup
RUN rm GodotTemplates.zip
RUN rm GodotSources.zip

# --- Godot Compilation ---
# Compile the editor.
#FROM GodotDownloader AS GodotCompiler

# Download required libraries.
RUN apt install build-essential scons pkg-config libx11-dev libxcursor-dev libxinerama-dev \
    libgl1-mesa-dev libglu-dev libasound2-dev libpulse-dev libfreetype6-dev libssl-dev libudev-dev \
    libxi-dev libxrandr-dev -y

# Compile the mono glue generator.
WORKDIR  /tmp/godot/sources
RUN ls
RUN scons p=server tools=yes bits=64 module_mono_enabled=yes mono_glue=no -j $(nproc)

# Generate mono glue.
RUN ./bin/godot_server.server.tools.64.mono --generate-mono-glue modules/mono/glue

# Compile with mono support.
RUN scons p=server tools=yes bits=64 module_mono_enabled=yes -j $(nproc)

# Tidy up the executables.
RUN mkdir -p /srv/godot/bin
RUN mv ./bin/godot_server.server.tools.64.mono /srv/godot/bin/godot
RUN mv ./bin/GodotSharpTools.dll /srv/godot/bin/
RUN mv ./bin/mscorlib.dll /srv/godot/bin/

# Copy the templates over.
RUN mkdir -p ~/.local/share ~/.config ~/.cache
RUN mkdir -p ~/.local/share/godot/templates/
RUN mv /tmp/godot/templates/* ~/.local/share/godot/templates/

# Add the executable to the path.
ENV PATH="/srv/godot/bin:${PATH}"
