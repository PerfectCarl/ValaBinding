# Vala addin for Monodevelop

ValaBinding is a Vala language binding for MonoDevelop.

# Features
* Vala project support for executable projects, libraries, and unit test projects (using GTest).
* Reference other Vala libraries and projects from within the IDE.
* Class browser.
* Basic Makefile integration.
* Breakpoints (via gdb)
* Full vala package management
* Using [echo](https://github.com/I-hate-farms/echo) for completion and code AST

# TODO 
* Full completion (code and packages, etc)
* Display vala variables in debug mode
* Find reference in code
* Basic refactoring (rename)
* More file and project templates (unit tests, elementary application/plugs, libpeas plugin, etc)
* Build the project via command line (make, cmake, hen, etc)

# Screenshots

![standard](docs/ide-monodevelop.png)

See more features [in action](docs/screenshots.md)
# How to install 
Not released or packaged yet. See #4 for more information

# How to build
First we need to install [Mono for ubuntu](http://www.mono-project.com/docs/getting-started/install/linux/#debian-ubuntu-and-derivatives")
```
sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update
sudo apt-get install mono-complete
```
Then we install Monodevelop
```
sudo apt-get install monodevelop
```
Then we build the vala plugin and its dependencies
```
sudo apt-get install monodevelop-nunit monodevelop-versioncontrol
sudo apt-get install libmono-addins-cil-dev libmono-addins-gui-cil-dev libmono-addins-gui0.2-cil libmono-addins-msbuild-cil-dev libmono-addins-msbuild0.2-cil libmono-addins0.2-cil mono-addins-utils

# Install spore ppa in not done already
curl -sL  http://i-hate-farms.github.io/spores/install | sudo bash -  
# Install project dependencies: echo
sudo apt-get install libecho-dev
```
And finally build the addin
```
git clone https://github.com/PerfectCarl/ValaBinding.git
cd ValaBinding
./autogen.sh --prefix=/usr && ./configure --prefix=/usr
make
sudo make install
```

