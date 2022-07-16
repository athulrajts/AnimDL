# AnimDL

# Usage
```
Usage:
  AnimDL [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  config          configure options for application
  grab <Title>    grabs stream links
  search <Title>  search anime on provider
  stream <Title>  stream anime
```

The `stream` option is disabled automatically if the project cannot find any of the supported streamers.

## `stream` / `grab`

These commands are the main set of command in the project. All of them scrape the target site, the only difference is how it is used.

- The `stream` option tosses the stream url to a player so that you can seamlessly binge your anime.
    - Streaming supports Discord Rich Presence with [`discord-rpc-csharp`](https://github.com/Lachee/discord-rpc-csharp).
    ```
    Description:
        stream anime

    Usage:
        AnimDL stream [<Title>] [options]

    Arguments:
        <Title>  Title of anime to search

    Options:
        -p, --provider <AnimeOut|AnimePahe|AnimixPlay|GogoAnime|Tenshi|Zoro>  provider name [default: AnimixPlay]
        -r, --range <range>                                                   range of episodes [default: 0..^0]
        --player <Vlc>                                                        media player to stream. [default: Vlc]
        -?, -h, --help                                                        Show help and usage information
    ```
- The `grab` option simply streams the stream url to the stdout stream.
    - This is useful for external usage and testing.
    ```
    Description:
        grabs stream links

    Usage:
        AnimDL grab [<Title>] [options]

    Arguments:
        <Title>  Title of anime to search

    Options:
        -p, --provider <AnimeOut|AnimePahe|AnimixPlay|GogoAnime|Tenshi|Zoro>  provider name [default: AnimixPlay]
        -r, --range <range>                                                   range of episodes [default: 0..^0]
        -?, -h, --help                                                        Show help and usage information
        ```

```sh
animdl stream "One Piece" 
```
<p align="center">
<sub>
Providers can be specified by using provider prefix, <code>stream "One Piece" -p gogoanime</code>, will use the 9Anime provider.
</sub></p>

## `-r` / `--range` argument
This argument is shared by **stream** and **grab**, it can be used to hand over custom ranges for selecting episodes.<br/>
uses c# range and indices syntax.