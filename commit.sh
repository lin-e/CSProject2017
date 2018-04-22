#!/bin/bash
git remote set-url origin git@github.com:lin-e/CSProject2017.git
git add -A
msg="$*"
git commit -am "$msg"
git push
curl https://project.eugenel.in/api/pull.php
