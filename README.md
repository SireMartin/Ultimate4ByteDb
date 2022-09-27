# Ultimate4ByteDb
Parses ETH Contract ABI and provides WebApp for The Ultimate 4-byte Function/Event Selector Database
The database provides probabilities for function/event signatures and argument names when hash clashes occur.

Currently this repo is an MVP for evaluation and contains 2 docker compose files.

# pre-parsed-redis.yml

Use this docker-compose te evaluate the WebApp => http://localhost:8080

The webapp back-end uses a redis database for key (=selectors) / value (=selector metadata json) lookup. 
The data is provided as a bind mount (dir ./redis_data) and contains the parsed Sourcify data of 21/09/2022.

Usage:
```
docker-compose -f pre-parsed-redis.yml up
```

## Troubleshooting
You might have on an issue running the redis container, as it is exposed to the host system on the default port 6379.
I expose this port to the host system for local development and to be able to use the redis-cli without options.
If you experience a problem you can 
- comment/remove the ports section of the redis service in the docker-compose file
- bind the redis container to another port on the host system
- stop the redis server on the host system

I experienced problems with the bind mount in a window environment. I simply does not bind the dump.rbp file without error notification.
I recommend running docker-compose on Linux or WSL (if Windows). I did not test it on Mac.

# parse-sourcify.yml

This setup parses all contract abi files in a given directory and writes the result to a redis database, which is used by a WebApp for presentation.

The abi parser looks recursively for files named "metadat.json" on a bind mount and adds the parsed data to an in memory data structure. 
At the end the data is written to a Redis database, which is persisted to a docker volume instead of a bind mount.
The rest of the setup is same as the setup described above for the pre-parsed-redis.yml.

Usage:
```
docker-compose -f parse-sourcify.yml up
```
Change the source dir (or introduce a symlink) of the bind mount of the abi_parser service to where your sourcify data is located at. The bind mount is set at ./sourcify_data on the host system.

The abi_parser service took about 2 hours for the complete Sourcify data on my i7 4th gen laptop with a 5400rpm HDD.

The abi_parser does not check on launch if there is already some data in the redis, so on every start of the docker-compose, the data will be parsed!

# Feedback

Please create issues on this repo if you found an error or are not happy with the layout or the (lack of some) functionality.

