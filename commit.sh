#!/bin/bash
git add -A
msg="$*"
git commit -am "$msg"
git push
curl http://project.eugenel.in/api/pull.php > /dev/null
