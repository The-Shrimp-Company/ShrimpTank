If you've found these documents, well done! We left these name files exposed so it
would be easier for us to change the random shrimp names, but this also makes it 
easier for anyone else to change them too!
There's a couple of things to note about these files if you do want to change the
names available though. First of all, the shrimp names file is in the order it's
in for a reason. We needed a lot of names, in case the player (you) had a lot of
shrimp, but a lot of the names seemed quite silly. So, what we did is we set the
file up in such a way that the names closer to the top get picked more regularly.
What this means is that if you want one of your shrimp names to be picked often,
you should put it at the top. Secondly, the file doesn't have any repeats in it
because we didn't want random names to repeat themselves. However, we also added
some checks for repeats in the code as well, so if you put repeated names into the
file, I don't know what will happen. (Sneaky tip though, we treat each line as a
seperate entry, so whitespace on the line is included in the name. If you can
figure out something to do with that information, good on you)

The NPC Email addresses are a lot less complex. They just take a random entry from
the file, and we have no checks for duplicates. The only way to make your names
show up more often would be to have it entered into the file more often.