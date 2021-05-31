# Tetris in C# and .net

this is the last assignment to finish this school subject.
The assignment is to recreate tetris in c# .net. 

## Table of contents

- [Team](#Team)
- [Git strategy](#git-strategy)
- [Documentation](#documentation)
- [Coding standards](#coding-standards)
    - [C#](#C#)
    - [Git](#git)


## The team

this assignment could be carried out by a maximum of two persons per group.
This repo belongs to Ruben and Milan.

- Milan Dol ([@JustMilan](https://github.com/JustMilan))
- Ruben van den Brink ([@Rubenvdbrink](https://github.com/Rubenvdbrink))

## Git strategy

Given that this is a short-term project and not very big either we've chosen to make issues and pick those up.
This project is way to short to create a whole backlog and sprint planning. 
We won't push to the `main branch` directly. Issues will be resolved by creating a `feature branch`. 
Pull request will always be checked and reviewed by the other person to ensure high code quality.

## Documentation

In the assignment it is clearly stated that all the code needs to be well documented. So this is what we'll be doing for the remainder of this project.
If the documentation hinders reading the actual code, in Rider you can tap `shift + ctrl + a`  and type `Collapse Doc Comments` this will, as it says, collapse all the doc comments.
Visual studio doens't have a build in feature for collapsing those comments although there are serveral plugins that can take care of this potential problem.

## Coding standards

This paragraph can give insights as to how we're keeping the quality of the code up.

### C#

To ensure the code quality of Java is high, we will implement certain patterns wherever needed and follow general rules that help keep our code clear.

We implement design patterns wherever possible, by using the explanations from the [refactoring.guru](https://refactoring.guru/) website.

Prefer `var` over explicitly typing your variable:

```c#
// Don't do this:
DispatcherTimer dispatcherTimer = new DispatcherTimer();

// Instead, do this:
var dispatcherTimer = new DispatcherTimer();
```

Remove unnecessary newlines:

```c#
// Don't do this:
public class Tetronimo
    {
    
        public int i { get; private set; }

    }

// Do this:
public class Tetronimo
    {
        public int i { get; private set; }
    }
```

Prefer Linq over multiple for-loops:

```c#
// Don't do this;
string matchString = "dsf897sdf78";
int matchIndex = -1;
for (var i=0; i < array.length; i++)
{
    if (array[i]==matchString)
    {
        matchIndex = i;
        break;
    }
}

// Do This:
int matchIndex = array.Select((r, i) => new { value = r, index = i })
                         .Where(t => t.value == matchString)
                         .Select(s => s.index).First();
```

Prefer to remove curly braces if that improves readability:

```c#
// Don't do this;
while (!reader.EndOfStream)
{
    foreach (var word in reader.ReadLine().Split(Delimiters))
    {
        if (predicate(word.ToLowerInvariant()))
        {
            yield return word;
        }
    }
}

// Do this:
while (!reader.EndOfStream)
    foreach (var word in reader.ReadLine().Split(Delimiters))
        if (predicate(word.ToLowerInvariant()))
            yield return word;
```

### Git

This has been discussed many times by other people, thus it is only natural to link a clear and concise article about this topic:

https://chris.beams.io/posts/git-commit/

The points mentioned in this article are the ones we should be using for making clear git messages.

To keep our git history clean, we don't use the `git merge` command by ourselves, instead, we rebase our `feature` branches. No unnecessary merges from `main` to `feature/...`, only merges from the PR's in `main`.
