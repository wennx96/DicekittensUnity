Hi, first of all: thank you for purchasing File Management.
I'm sure it will make your developer's life easier.

Then: Before start with the application, please extract the "ExtractMeToStreamingAssets.zip" file into the "StreamingAssets" folder.
The "StreamingAssets" folder is a special Unity fodler. It must be right in the "Assets" folder, otherwise Unity will not find it.

If you see a lot of errors when opening for first time, that should be you are using the wrong platform.
So to fix it you must open your "BuildSettings" dialog and chose any pltform but "WebPlayer" (Already deprecated platform).

Please take a look at the documentation.

Thanks again.
eToile

FOR WEBGL BUILDS !!!!!
If you experience problems with MIME types in WebGL, please create a "web.config" text file with this content,
right into the folder where "index.html" is placed:

<configuration>
	<system.webServer>
		<staticContent>
			<mimeMap fileExtension=".unityweb" mimeType ="TYPE/SUBTYPE" />
			<mimeMap fileExtension="" mimeType ="text/plain; charset=x-user-defined" />
		</staticContent>
	</system.webServer>
</configuration>