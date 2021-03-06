# sg-unpack
Unpacker for SciADV series .mpk content files. 

Although originally written for the Steam edition of STEINS;GATE, sg-unpack is known to work with the following:

| Game  | Source(s) |
| ------------- | ------------- |
| STEINS;GATE | Steam, DMM |
| STEINS;GATE 0 | Steam, DMM |
| STEINS;GATE: Linear Bounded Phenogram | Steam |
| Chaos;Child | Steam, DMM |

Other configurations might also work - including other games using the .mpk format - but these are untested. 

sg-unpack does not currently implement the extraction logic required to unpack STEINS;GATE ELITE .cpk files. 

### Downloading
Check the [releases](https://github.com/rdavisau/sg-unpack/releases) page for the latest version.

### Usage
`sg-unpack` is a command line tool that takes one or many .mpk inputs and extracts the content to a given output directory.

```
sg-unpack 1.0.0.0
Copyright © Ryan Davis 2017

  -i, --input-path       Required. Path to input/s. Can be an individual file, or a directory.

  -m, --input-mask       (Default: *.mpk) Search mask for --input-path, used if --input-path is a directory.

  -o, --output-path      Output path for extracted files. If ommitted, contents will be listed but not extracted.

  -f, --output-filter    Case-insensitive filter for files to extract.

  -q, --quiet            (Default: false) Set true to hide most output.

  --help                 Display this help screen.

  --version              Display version information.
```

### Examples

#### List the files in the `bgm.mpk` pack by omitting the `output-path`:

###### Command
`sg-unpack.exe -i C:\Program Files (x86)\Steam\steamapps\common\STEINS;GATE\USRDIR\bgm.mpk`

###### Output
```
Using inputs [ bgm.mpk ] in C:\Program Files (x86)\Steam\steamapps\common\STEINS;GATE.

Scanning 1 inputs...
Processing 83 entries...
bgmBGM03.ogg      ~4.87 MB
bgmBGM04.ogg      ~3.49 MB
         * snip *
bgmBGM999.ogg     ~2.53 MB
bgmSONG1.ogg      ~2.45 MB

83 entries found in 1 pack.
No files extracted. Use --output-path to specify an output directory.
```

#### Use output filter to extract all audio files (`.ogg`) from the mpk files in the content directory to `C:\SG`, without listing the processed files (`-q true`):

###### Command
`sg-unpack.exe -i C:\Program Files (x86)\Steam\steamapps\common\STEINS;GATE\USRDIR -f .ogg -o C:\SG -q true`

###### Output
```
Using inputs [ bg.mpk, bgm.mpk, chara.mpk, manual.mpk, mask.mpk, mgsshader.mpk, script.mpk, se.mpk, shader.mpk, system.mpk, voice.mpk ] in C:\Program Files (x86)\Steam\steamapps\common\STEINS;GATE.

Scanning 11 inputs...
Processing 16,096 entries matching on filter '.ogg'...

14,794 files (1.79 GB) extracted to 'C:\SG' in 00:00:07.7275492.
```

#### Extract All The Things:

###### Command
`sg-unpack.exe -i C:\Program Files (x86)\Steam\steamapps\common\STEINS;GATE\USRDIR -o C:\SG`

###### Output
```
Using inputs [ bg.mpk, bgm.mpk, chara.mpk, manual.mpk, mask.mpk, mgsshader.mpk, script.mpk, se.mpk, shader.mpk, system.mpk, voice.mpk ] in C:\Program Files (x86)\Steam\steamapps\common\STEINS;GATE.

Scanning 11 inputs...
Processing 16,096 entries...

* snip sixteen thousand lines *

16,096 files (3.31 GB) extracted to 'C:\SG' in 00:00:12.2559171.
```
