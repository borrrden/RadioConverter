# Programmable Radio Format Converter

In a world where programmable Ham radio data formats are fragmented and inconsistent, this project seeks to provide a way to convert between them.  Instead of predefining formats, the goal here is to provide a way to specify transforms from an input to an output.

## Current Input Model

The current working model for input is based on the Wireless Institute of Australia's repeater directory.  If there is enough interest then other formats can be explored but to keep things relatively simple one input is being focused on.  The up to date version can be retrieved from the "Latest repeater list CSV" download on [this page](https://www.wia.org.au/members/repeaters/data/).

## Defining Output Formats

The format for specifying transforms is a format called [YAML](https://yaml.org/).  It is a simple format that serves the purposes of this project well.  A quick primer on YAML is that it breaks down information into primitives (like words and numbers) and collections.  The two collection types are:

- Associative Array: A collection containing key/value pairs inside (useful if you want to say "X is Y" or "X has Y")
- Array: A collection containing X number of items inside (useful if you want to say "I have several of X")

A quick example:

```yaml
species: dog
name: Fluffy
age: 3
children: 
  - species: dog
    name: Pooky
    age: 1
  - species: dog
    name: Chuckles
    age: 1
```

This describes a top level associate array ("species is dog", "name is Fluffy", "age is 3") and then has an array as the value in the key "children" ("Fluffy has 2 children").  Each of the children are, in turn, also associative arrays ("species is dog", "name is Pooky", "age is 1").

For the purposes of defining output formats, the file will always start as follows (Anything inside of `<>` is in lieu of an actual value):

```yaml
output: 
  columns:
    - name: <output name>
      type: <transform type>
    - name: <output name>
      type: <transform type>
    - name: <output name>
      type: <transform type>
```

What this is saying is "output has columns", of which there are several.  Each of these columns is going to be some sort of transform that acts on the input and produces an output.  So far this project has defined four types of transform.

### Constant Transform

```yaml
type: constant
value: <...>
```

This is a very simple transform, it will just take whatever is in value, and put that into the output (it must be a primitive, and not a collection)

### Increment Transform

```yaml
type: increment
start: <number>
```

This transform will first output start, and then for every new row it will increment the number by 1 before outputting again.  Useful for something like memory slots, where the output should be 1, 2, 3, 4, etc.

### Mapping Transform

```yaml
type: mapping
source: <input column>
```

This transform will take the value from an input column (defined in source) and write it into an output column.  So say your input file has the column 'Call' with a callsign, but your output format wants to write that information into 'Name' instead.  That is what this is for.  

### Lua Transform

```yaml
type: lua
inputs: 
    - name: <output name>
      type: <transform type>
    - name: <output name>
      type: <transform type>
script: |
   function Calculate(input1, input2)
      <your logic>
   end
```

This is the most powerful transform.  It will take all the inputs that you define as inputs (usually mapping transforms) and run them through the script that you define in script.  This affords you the opportunity to do a lot of specific comparisons that would otherwise be overly verbose to try to express otherwise.  Take note that the function above _must_ be named Calculate, and the number of inputs must match the number of parameters in the function.  The names of the parameters are not important to the program, so you can name them something that will make sense to you.