[license_image]: https://badgen.net/badge/license/MIT/blue
[license_url]: https://github.com/valapi/.github/blob/main/LICENSE
[github_image]: https://badgen.net/badge/icon/github?icon=github&label
[github_url]: https://github.com/valapi/node-valapi
[discord_image]: https://badgen.net/badge/icon/discord?icon=discord&label
[discord_url]: https://discord.gg/pbyWbUYjyt

<div align="center">

# Wey

### wey is the way to share minecraft mods

(for Fabric Loader)

[![LICENSE][license_image]][license_url]
[![Github][github_image]][github_url]
[![Discord][discord_image]][discord_url]

</div>

---

> wéy (Indo-European) means "we"

## Install Mods

1st Method: **Local Create**

```bash
wey create
wey add modrinth|curseforge
wey load
```

2nd Method: **Import**

```bash
wey import
wey update
wey load
```

## Mods IDs

<details>
<summary>Modrinth</summary><br>

![image](https://github.com/user-attachments/assets/b3815666-fdb3-40ea-9fb9-f4ca49fe456a)

</details>

<details>
<summary>Curseforge</summary><br>

![image](https://github.com/user-attachments/assets/e27bce82-21b9-4fc7-9493-2d44dbaea195)

</details>

## Commands

<details>
<summary>create</summary><be>

create new profile

Arguments:
  
- gameVersion (string)
- name (string)

</details>

<details>
<summary>import</summary><be>

import profile

Arguments:
  
- path (string or URL)

</details>

<details>
<summary>add [curseforge|modrinth]</summary><be>

add mods to the profile

Arguments:
  
- name (string)
- ids (string[])
- curseforgeApi (string)

Curseforge API Key:

https://console.curseforge.com/?#/api-keys

You must log in first

![image](https://github.com/user-attachments/assets/8c3c6049-51b8-46eb-8eeb-65b1068598ee)

</details>

<details>
<summary>update</summary><be>

update mods in the profile

Arguments:
  
- name (string)

</details>
 
<details>
<summary>load</summary><be>

download mods from the profile and create minecraft launcher profile

Arguments:
  
- name (string)

</details>
