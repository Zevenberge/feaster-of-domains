find . -name "*.csproj" -print0 \
    | tar -cvf projectfiles.tar --null -T -