# FileSystemWatcherConsole

A .NET application that provides a flexible and extensible way to perform file system operations with support for file watching, locking, and sequential processing. The application uses a JSON-based configuration to define sequences of file operations.

## Features

- **File Watching**: Monitor file system changes
- **File Operations**: Copy, move, delete files and folders
- **File Locking**: Lock files temporarily or until user interaction
- **Template Processing**: Create and update files from templates
- **User Interaction**: Pause and resume operations with key press
- **Structured Logging**: Comprehensive logging of all operations to the console and log file
- **JSON Configuration**: Define operation sequences in JSON

## Getting Started

1. Clone the repository / download release
2. Configure your action sequence in a JSON file (in case you need actions)
3. Run the application

## Command line options
Usage:
  FileSystemWatcherConsole `path` [options]

Arguments:
  `path`  The directory to watch.

Options:
  --actions `actions.json`  The json file to process actions.

### Action Sequences

Actions are defined in JSON files. Each action has:
- `processorType`: The type of operation to perform
- `enabled`: Whether the action should be executed
- `name`: A descriptive name for the action
- Additional parameters specific to the action type

## Available Actions

1. **CopyFile**: Copy a single file
2. **CopyFolder**: Copy a folder and its contents
3. **MoveFolder**: Move a folder to a new location
4. **CopyFileWithLock**: Copy a file and maintain a lock for a specified duration
5. **DeleteFile**: Delete a single file
6. **DeleteFolder**: Delete a folder and its contents
7. **LockFile**: Lock a file for a specified duration
8. **LockFileUntilKeyPressed**: Lock a file until user presses a key
9. **WaitKey**: Pause execution until user presses a key
10. **CreateAndUpdateFileFromTemplate**: Create a file from a template and update it

## Example Scenarios

The repository includes several example action sequences:

### [Project Setup](examples/project-setup.json)
Demonstrates setting up a project structure:
- Copies template folder
- Waits for user confirmation
- Copies and locks configuration file
- Generates and updates README file

### [Cleanup Operations](examples/cleanup.json)
Shows a cleanup workflow with user interaction:
- Asks for confirmation
- Locks database during cleanup
- Removes temporary files and old logs
- Confirms completion

### [Data Update Process](examples/data-update.json)
Illustrates a data update workflow:
- Backs up current data
- Moves new data into place
- Updates version information
- Temporarily locks files during update

## Usage Examples
1. Just watching for file system events:
```bash
./FileSystemWatcherConsole ./examples
```

2. Watching for file system events and run specific actions:
```bash
./FileSystemWatcherConsole ./examples --actions examples/project-setup.json
```

## Best Practices

1. Always test action sequences with `enabled: false` first
2. Use `WaitKey` actions for critical points that need verification
3. Include meaningful action names for better logging
4. Use file locks appropriately to prevent conflicts
5. Implement proper error handling in your action sequences

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License
